export class GetOrdersUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute() {
    return this.apiClient.request("/orders");
  }
}
