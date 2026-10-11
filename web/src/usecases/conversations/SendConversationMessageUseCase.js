export class SendConversationMessageUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(conversationId, text) {
    return this.apiClient.request(`/conversations/${conversationId}/messages`, {
      method: "POST",
      body: { text },
    });
  }
}
