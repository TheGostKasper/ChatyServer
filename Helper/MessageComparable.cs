using signalr_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_server.Helper
{
    public class MessageComparable : IEqualityComparer<Message>
    {
        public bool Equals(Message x, Message y)
        {
            return x.ToUserId == y.ToUserId && x.FromUserId == y.ToUserId;
        }

        public int GetHashCode(Message obj)
        {
            return obj.ToUserId.GetHashCode();
        }
    }
}
