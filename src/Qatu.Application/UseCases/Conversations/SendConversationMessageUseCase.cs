using Qatu.Domain;

namespace Qatu.Application;

public sealed class SendConversationMessageUseCase(
    IWriteRepository writer,
    IUnitOfWork unitOfWork,
    ConversationAccessPolicy conversations)
{
    public async Task<ChatMessage> ExecuteAsync(SendConversationMessageCommand command)
    {
        await conversations.GetForParticipantAsync(command.UserId, command.ConversationId);
        if (string.IsNullOrWhiteSpace(command.Text) || command.Text.Trim().Length > 1000)
            throw MarketErrors.Invalid("El mensaje debe tener entre 1 y 1000 caracteres.");

        var message = new ChatMessage
        {
            ConversationId = command.ConversationId,
            SenderId = command.UserId,
            Text = command.Text.Trim()
        };
        writer.Add(message);
        await unitOfWork.SaveAsync();
        return message;
    }
}
