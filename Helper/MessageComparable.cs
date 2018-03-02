using signalr_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_server.Helper
{
    public class MessageComparable : IEqualityComparer<MessageObj>
    {
        public bool Equals(MessageObj x, MessageObj y)
        {
            return
                (x.ToUserId == y.ToUserId && x.FromUserId == y.ToUserId)
                || (x.ToUserId == y.FromUserId && x.FromUserId == y.ToUserId);
        }

        public int GetHashCode(MessageObj obj)
        {
            return obj.ToUserId.GetHashCode() ^ obj.FromUserId.GetHashCode();
        }
    }
}
