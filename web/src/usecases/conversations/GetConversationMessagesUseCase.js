export class GetConversationMessagesUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(conversationId) {
    return this.apiClient.request(`/conversations/${conversationId}/messages`);
  }
}
