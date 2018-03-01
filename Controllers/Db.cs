using signalr_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_server.Controllers
{
    public static class Db
    {
        public static HashSet<User> users = new HashSet<User>();
        public static HashSet<Message> messages = new HashSet<Message>();
    }
}
