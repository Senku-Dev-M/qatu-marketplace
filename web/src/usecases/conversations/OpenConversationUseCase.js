export class OpenConversationUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(productId) {
    return this.apiClient.request("/conversations", {
      method: "POST",
      body: { productId },
    });
  }
}
