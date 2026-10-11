using Qatu.Domain;

namespace Qatu.Application;

public sealed class GetConversationMessagesUseCase(
    IReadRepository reader,
    ConversationAccessPolicy conversations)
{
    public async Task<List<ChatMessage>> ExecuteAsync(GetConversationMessagesQuery query)
    {
        await conversations.GetForParticipantAsync(query.UserId, query.ConversationId);
        return (await reader.ListAsync<ChatMessage>(message =>
                message.ConversationId == query.ConversationId))
            .OrderBy(message => message.SentAt)
            .ToList();
    }
}
