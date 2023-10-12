using gRPChat.Database.Models;

namespace gRPChat.Backend
{
    public class ChatRoomManager
    {
        public event Action<string>? MessageSended;

        public async Task AddMessageAsync(ChatMessage chatMessage)
        {
            MessageSended?.Invoke(chatMessage.Message);
        }
    }
}
