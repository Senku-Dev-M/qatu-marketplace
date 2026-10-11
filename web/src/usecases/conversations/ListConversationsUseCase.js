export class ListConversationsUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute() {
    return this.apiClient.request("/conversations");
  }
}
