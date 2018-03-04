using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using signalr_server.Context;
using signalr_server.Models;

namespace signalr_server
{
    public class ChatHub : Hub
    {
        private readonly ChatyDb _context;
        public ChatHub(ChatyDb context)
        {
            _context = context;
        }

        public Task Joinser(string message)
        {
            return Clients.All.InvokeAsync("joinser", message);
        }

        public Task NewUser(User user)
        {
            user.ConnectionId = Context.ConnectionId;
            return Clients.All.InvokeAsync("newUser", user);
        }

        public string GetConnctionId()
        {
            return Context.ConnectionId;
        }
        public void ChangeStatus(int userId)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
            currentUser.Status = !currentUser.Status;

            if (currentUser.Status) currentUser.ConnectionId = "";


            Clients.AllExcept(getExceptions()).InvokeAsync("changeStatus", currentUser);
        }

        public void UpdateConnectionId(int userId)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
            currentUser.ConnectionId = Context.ConnectionId; // String.Format("{0},{1}", currentUser.ConnectionId, Context.ConnectionId);
            _context.SaveChanges();
            Clients.AllExcept(getExceptions()).InvokeAsync("changeStatus", currentUser);
        }

        public void SendPM([Microsoft.AspNetCore.Mvc.FromBody]Message message)
        {

            //var connections = Db.users.FirstOrDefault(u => u.Id == message.ToUserId).ConnectionId.Split(",");
            //foreach (var item in connections)
            //{
            //    Clients.Client(item).InvokeAsync("sendPM", message);
            //}
            try
            {
                Clients.Client(GetUserConnection(message.ToUserId).ConnectionId).InvokeAsync("sendPM", new
                {
                    message.Content,
                    message.Status,
                    user = message.ToUser
                });
                message.CreationDate = DateTime.UtcNow;
                _context.Messages.Add(message);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public User GetUserConnection(int userId)
        {
            return _context.Users.FirstOrDefault(e => e.UserId == userId);

        }
        public void ReadMessages([Microsoft.AspNetCore.Mvc.FromBody]RequestDTO request)
        {
            var list = _context.Messages.Where(e =>
              ((e.FromUserId == request.FromUser && e.ToUserId == request.ToUser) ||
              (e.FromUserId == request.ToUser && e.ToUserId == request.FromUser)) && e.Status != true).ToList();

            list.ForEach(msg => msg.Status = true);

            _context.SaveChanges();
            Clients.Client(GetUserConnection(request.ToUser).ConnectionId).InvokeAsync("read",request.FromUser);
        }
        public void HandleException(Exception ex)
        {

        }
        public IReadOnlyList<string> getExceptions(params string[] _params)
        {
            IReadOnlyList<string> allExcept = new List<string>();
            foreach (var item in _params)
            {
                allExcept.Append(item);
            }
            allExcept.Append(Context.ConnectionId);
            return allExcept;
        }
    }
}
