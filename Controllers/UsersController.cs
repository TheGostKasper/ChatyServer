using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using signalr_server.Context;
using signalr_server.Helper;
using signalr_server.Models;

namespace signalr_server.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ChatyDb _context;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly HelperServices helperServices;
        private readonly IHostingEnvironment _hostingEnvironment;
        public UsersController(ChatyDb context, IHubContext<ChatHub> chatHub,
            Microsoft.Extensions.Configuration.IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _chatHub = chatHub;
            _hostingEnvironment = hostingEnvironment;
            helperServices = new HelperServices(configuration);
        }


        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                var _user = _context.Users.SingleOrDefault(e => e.Password == user.Password && e.Email == user.Email);

                if (_user != null) return Ok(new
                {
                    token = helperServices.GetToken(_user),
                    message = "Welcome back",
                    data = _user
                });
                return Ok(new
                {
                    message = "Email or password is uncorrect",
                    data = new { }
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("Friends")]
        public IActionResult Friends()
        {
            try
            {
                return Ok(new
                {
                    message = "OK",
                    data = _context.Users.Select(e => new
                    {
                        e.ConnectionId,
                        e.UserId,
                        e.Name,
                        e.Status,
                        e.Avatar
                    }).ToList()
                });
            }
            catch (Exception err)
            {
                return Ok(new
                {
                    message = err,
                    data = new { }
                });
            }
        }
        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            try
            {
                if (id == user.UserId)
                    _context.Entry(user).State = EntityState.Modified;
                else return BadRequest(ModelState);
                // (CheckEmailAvailability(user.Email))

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(new
            {
                data = user,
                message = "User updated successfully"
            });
        }

        // POST: api/Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (CheckEmailAvailability(user.Email)) return Ok(new
            {
                message = "Email you entered already exist",
                data = new { }
            });

            user.Status = true;
            user.CreationDate = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _chatHub.Clients.All.InvokeAsync("NewUser", user);


            var token = helperServices.GetToken(user);
            var currUser = await _context.Users.SingleOrDefaultAsync(m => m.Email == user.Email);

            return Ok(new
            {
                token,
                data = currUser
            });


        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
        [HttpPost]
        [Route("uploadAvatar")]
        public string UploadAvatar([FromBody] User user)
        {
            try
            {
                string file = String.Format("Images/{0}.jpg", user.UserId);
                string hostFilePath = String.Format("{0}://{1}/{2}", (HttpContext.Request.IsHttps)?"https":"httplogin", HttpContext.Request.Host.ToUriComponent(), file);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, file);

                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(user.Avatar));
                var curruser = _context.Users.FirstOrDefault(e => e.UserId == user.UserId);
                if (curruser != null)
                {
                    curruser.Avatar = hostFilePath;
                    _context.SaveChanges();
                }
                return hostFilePath;
            }
            catch (Exception ex)
            {
                return ex.InnerException.ToString();
            }

        }


        [HttpPost]
        [Route("search")]
        public IActionResult SearchUsers([FromBody]RequestDTO request)
        {
            try
            {
                var results = _context.Users.Where(e => e.Email.Contains(request.Search) || e.Name.Contains(request.Search))
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.Page * request.PageSize)
                    .OrderBy(e => e.UserId)
                    .ToList();

                return Ok(new {
                    data = results,
                    message ="Here your users"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    data = new { },
                    message = ex.InnerException.ToString()
                });
            }
        }
        public bool CheckEmailAvailability(string email)
        {
            var exist = _context.Users.FirstOrDefault(e => e.Email == email);
            return (exist != null) ? false : true;
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}