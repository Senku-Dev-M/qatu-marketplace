export class GetProductDetailsUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(productId) {
    return this.apiClient.request(`/products/${productId}`);
  }
}
