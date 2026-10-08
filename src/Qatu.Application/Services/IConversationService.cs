using Qatu.Domain;

namespace Qatu.Application;

public interface IConversationService
{
    Task<Conversation> OpenAsync(Guid buyerId, Guid productId);
    Task<List<Conversation>> ConversationsAsync(Guid userId);
    Task<Conversation> GetAsync(Guid userId, Guid id);
    Task<List<ChatMessage>> MessagesAsync(Guid userId, Guid conversationId);
    Task<ChatMessage> SendAsync(Guid userId, Guid conversationId, string text);
}
