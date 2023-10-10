using gRPChat.Database.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace gRPChat.Database
{
    public class ChatDbContext : IdentityDbContext<ChatUser>
    {
        public DbSet<ChatMessage> Messages { get; set; }
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        protected ChatDbContext()
        {
        }
    }
}