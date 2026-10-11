export class CreateReviewUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(orderId, values) {
    return this.apiClient.request("/reviews", {
      method: "POST",
      body: {
        orderId,
        kind: values.kind,
        stars: Number(values.stars),
        comment: values.comment,
      },
    });
  }
}
