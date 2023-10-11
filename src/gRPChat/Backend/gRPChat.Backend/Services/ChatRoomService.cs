using gRPChat.Protos;
using Grpc.Core;
using ChatMessage = gRPChat.Protos.ChatMessage;
using gRPChat.Database;

namespace gRPChat.Backend
{
    public class ChatRoomService : ChatRoom.ChatRoomBase
    {
        private readonly ChatRoomManager _chatRoomManager;
        private readonly ChatDbContext _chatDbContext;

        private List<IServerStreamWriter<ChatMessage>> _listeners = new();

        public ChatRoomService(ChatRoomManager chatRoomManager, ChatDbContext chatDbContext)
        {
            _chatRoomManager = chatRoomManager;
            _chatDbContext = chatDbContext;

            _chatRoomManager.MessageSended += ChatRoomService_MessageSended;
        }


        public override async Task JoinChat(ChatRequest request, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            foreach (var chatMessage in _chatDbContext.Messages)
            {
                await responseStream.WriteAsync(new ChatMessage { Message = chatMessage.Message });
            }

            _listeners.Add(responseStream);

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            _listeners.Remove(responseStream);
        }

        public override async Task<ChatRequest> Send(ChatMessage request, ServerCallContext context)
        {
            var chatMessage = new Database.Models.ChatMessage
            {
                Message = request.Message
            };

            await _chatRoomManager.AddMessageAsync(chatMessage);

            await _chatDbContext.Messages.AddAsync(chatMessage);
            await _chatDbContext.SaveChangesAsync();

            return new ChatRequest();
        }

        private void ChatRoomService_MessageSended(string message)
        {
            foreach (var streamWriter in _listeners)
            {
                streamWriter.WriteAsync(new ChatMessage
                {
                    Message = message
                });
            }
        }
    }
}
