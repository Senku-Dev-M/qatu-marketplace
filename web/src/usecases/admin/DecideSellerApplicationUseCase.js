export class DecideSellerApplicationUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(applicationId, approve) {
    return this.apiClient.request(
      `/seller-applications/${applicationId}/decision`,
      { method: "POST", body: { approve } },
    );
  }
}
