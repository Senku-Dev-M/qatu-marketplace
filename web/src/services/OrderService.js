export class OrderService {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  list() {
    return this.apiClient.request("/orders");
  }

  buy(productId, quantity, paymentSucceeds) {
    return this.apiClient.request("/orders", {
      method: "POST",
      body: { productId, quantity, paymentSucceeds },
    });
  }

  review(orderId, values) {
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
