namespace Qatu.Application;

public sealed record GetConversationMessagesQuery(Guid UserId, Guid ConversationId);
