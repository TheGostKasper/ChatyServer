using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_server.Models
{
    public class MessageObj
    {
        public int MessageId { get; internal set; }
        public int ToUserId { get; internal set; }
        public int FromUserId { get; internal set; }
        public User FromUser { get; internal set; }
        public string Content { get; internal set; }
    }
}
