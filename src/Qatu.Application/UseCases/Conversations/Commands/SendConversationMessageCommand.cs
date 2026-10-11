namespace Qatu.Application;

public sealed record SendConversationMessageCommand(Guid UserId, Guid ConversationId, string Text);
