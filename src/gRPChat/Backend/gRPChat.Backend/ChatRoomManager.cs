using gRPChat.Database;
using gRPChat.Database.Models;

namespace gRPChat.Backend
{
    public class ChatRoomManager
    {
        private readonly ChatDbContext _chatDbContext;

        public event Action<string> MessageSended;

        public ChatRoomManager(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }

        public IEnumerable<ChatMessage> GetMessages()
        {
            return _chatDbContext.Messages;
        }

        public async Task AddMessageAsync(ChatMessage chatMessage)
        {
            await _chatDbContext.Messages.AddAsync(chatMessage);
            await _chatDbContext.SaveChangesAsync();

            MessageSended?.Invoke(chatMessage.Message);
        }
    }
}
