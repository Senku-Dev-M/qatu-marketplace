export class ApplyAsSellerUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(application) {
    return this.apiClient.request("/seller-applications", {
      method: "POST",
      body: application,
    });
  }
}
