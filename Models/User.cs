using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_server.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        public string Avatar { get; set; }
        public bool Status { get; set; }
        public DateTime? CreationDate { get; set; }

    }
}
