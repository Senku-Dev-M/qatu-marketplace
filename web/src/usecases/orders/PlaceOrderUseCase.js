export class PlaceOrderUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(productId, quantity, paymentSucceeds) {
    return this.apiClient.request("/orders", {
      method: "POST",
      body: { productId, quantity, paymentSucceeds },
    });
  }
}
