export class LoadSellerWorkspaceUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  async execute() {
    const [shop, products] = await Promise.all([
      this.apiClient.request("/shops/mine"),
      this.apiClient.request("/products/mine"),
    ]);
    return { shop, products };
  }
}
