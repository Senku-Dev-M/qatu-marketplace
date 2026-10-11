using Qatu.Domain;

namespace Qatu.Application;

public sealed class AuthorizeConversationUseCase(ConversationAccessPolicy conversations)
{
    public Task<Conversation> ExecuteAsync(Guid userId, Guid conversationId) =>
        conversations.GetForParticipantAsync(userId, conversationId);
}
