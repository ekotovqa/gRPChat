using Grpc.Core;
using gRPChat.Database;
using gRPChat.Database.Models;
using gRPChat.Protos;
using Microsoft.AspNetCore.Identity;
using ChatMessage = gRPChat.Protos.ChatMessage;

namespace gRPChat.Backend
{
    public class ChatRoomService : ChatRoom.ChatRoomBase
    {
        private readonly ChatRoomManager _chatRoomManager;
        private readonly ChatDbContext _chatDbContext;
        private readonly UserManager<ChatUser> _userManager;

        private List<IServerStreamWriter<ChatMessage>> _listeners = new();

        public ChatRoomService(ChatRoomManager chatRoomManager, ChatDbContext chatDbContext, UserManager<ChatUser> userManager)
        {
            _chatRoomManager = chatRoomManager;
            _chatDbContext = chatDbContext;
            _userManager = userManager;

            _chatRoomManager.MessageSended += ChatRoomService_MessageSended;
        }


        public override async Task JoinChat(ChatRequest request, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            foreach (var chatMessage in _chatDbContext.Messages)
            {
                await responseStream.WriteAsync(new ChatMessage { Message = chatMessage.Message });
            }

            _listeners.Add(responseStream);

            await Task.Delay(int.MaxValue, context.CancellationToken);

            _listeners.Remove(responseStream);
        }

        public override async Task<ChatRequest> Send(ChatMessage request, ServerCallContext context)
        {
            var user = await _userManager.GetUserAsync(context.GetHttpContext().User);

            var chatMessage = new Database.Models.ChatMessage
            {
                Message = request.Message,
                User = user
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
