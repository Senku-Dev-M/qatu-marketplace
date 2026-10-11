export class ChangeProductStatusUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(productId, status) {
    return this.apiClient.request(`/products/${productId}/status`, {
      method: "POST",
      body: { status },
    });
  }
}
