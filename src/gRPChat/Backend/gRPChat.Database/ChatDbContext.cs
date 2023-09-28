using gRPChat.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace gRPChat.Database
{
    public class ChatDbContext : DbContext
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