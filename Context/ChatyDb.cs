using Microsoft.EntityFrameworkCore;
using signalr_server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalr_server.Context
{
    public class ChatyDb : DbContext
    {
        public ChatyDb(DbContextOptions<ChatyDb> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
