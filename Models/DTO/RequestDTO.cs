using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_server.Models
{
    public class RequestDTO
    {
        public int Page { get; set; }
        public int FromUser { get;  set; }
        public int ToUser { get;  set; }
        public int PageSize { get;  set; }
        public string Email { get; set; }
        public string Search { get;  set; }
    }
}
