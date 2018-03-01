using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_server.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string Content { get; set; }
        public bool Status { get; set; }

        // public List<string> Files { get; set; }
        public int FromUserId { get; set; }
        [ForeignKey("ToUser")]
        public int ToUserId { get; set; }
        public User ToUser { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
