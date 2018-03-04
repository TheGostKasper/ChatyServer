using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using signalr_server.Context;
using signalr_server.Helper;
using signalr_server.Models;

namespace signalr_server.Controllers
{
    [Produces("application/json")]
    [Route("api/Messages")]
    public class MessagesController : Controller
    {
        private readonly ChatyDb _context;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly HelperServices helperServices;
        public MessagesController(ChatyDb context, IHubContext<ChatHub> chatHub, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _context = context;
            _chatHub = chatHub;
            helperServices = new HelperServices(configuration);
        }
        [HttpGet]
        [Route("Init")]
        public IActionResult Init(int userId)
        {
            try
            {
                var _users = _context.Users.Select(e => new User { UserId = e.UserId, Name = e.Name }).ToList();
                var oldMessages = _context.Messages
                .Where(m => m.FromUserId == userId || m.ToUserId == userId)
                .Select(e => new MessageObj
                {
                    MessageId = e.MessageId,
                    ToUserId = e.ToUserId,
                    FromUserId = e.FromUserId,
                    User = _users.FirstOrDefault(fu => fu.UserId == ((e.ToUserId == userId) ? e.FromUserId : e.ToUserId)),
                    Content = e.Content,
                    Status = e.Status
                })
                .Distinct()
                .GroupBy(e => new { e.ToUserId, e.FromUserId })
                .ToList();
                return Ok(new
                {
                    data = Chatters(GetChatters(oldMessages)),
                    message = "Enjoy youe friends suckers"
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("Notification")]
        public IActionResult Notification(int userId)
        {
            try
            {
                var _users = _context.Users.ToList();
                var notifications = _context.Messages.Where(m => m.ToUserId == userId && m.Status == false)
                    .Select(e => new MessageObj
                    {
                        MessageId = e.MessageId,
                        ToUserId = e.ToUserId,
                        FromUserId = e.FromUserId,
                        User = _users.FirstOrDefault(fu => fu.UserId == e.FromUserId),
                        Content = e.Content,
                        Status = e.Status
                    })
                    .OrderBy(e => e.MessageId)
                    .Distinct()
                    .GroupBy(e => e.FromUserId)
                    .ToList();

                return Ok(new
                {
                    data = GetChatters(notifications),
                    message = "Here what you have missed"
                });
                // return Ok(notifications);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<MessageObj> GetChatters<T>(List<IGrouping<T, MessageObj>> objs)
        {
            var lst = new List<MessageObj>();
            objs.ForEach(e =>
            {
                var res = e.TakeLast(1).Select(msg => msg).FirstOrDefault();
                lst.Add(res);
            });
            return lst.OrderByDescending(e => e.MessageId).ToList();
        }

        public List<MessageObj> Chatters(List<MessageObj> lst)
        {
            var res = new HashSet<MessageObj>(lst, new MessageComparable());
            return res.ToList();
        }
        [HttpPost]
        [Route("ListMessages")]
        public IActionResult ListMessages([FromBody] RequestDTO req)
        {
            try
            {
                var messages = _context.Messages.Where(e =>
                  (e.FromUserId == req.FromUser && e.ToUserId == req.ToUser) || (e.FromUserId == req.ToUser && e.ToUserId == req.FromUser))
                  .Skip((req.Page - 1) * req.PageSize)
                  .OrderByDescending(e=>e.MessageId)
                  .Take(req.Page * req.PageSize)
                  .ToList();
                return Ok(new
                {
                    data = messages,
                    message = "Here your prev messages"
                });
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}