export class CreateProductUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(product) {
    return this.apiClient.request("/products", {
      method: "POST",
      body: product,
    });
  }
}
