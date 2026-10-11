export class UpdateProductUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(productId, product) {
    return this.apiClient.request(`/products/${productId}`, {
      method: "PUT",
      body: product,
    });
  }
}
