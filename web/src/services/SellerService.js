export class SellerService {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  apply(data) {
    return this.apiClient.request("/seller-applications", {
      method: "POST",
      body: data,
    });
  }

  async workspace() {
    const [shop, products] = await Promise.all([
      this.apiClient.request("/shops/mine"),
      this.apiClient.request("/products/mine"),
    ]);
    return { shop, products };
  }

  save(id, data) {
    return this.apiClient.request(id ? `/products/${id}` : "/products", {
      method: id ? "PUT" : "POST",
      body: data,
    });
  }

  setStatus(id, status) {
    return this.apiClient.request(`/products/${id}/status`, {
      method: "POST",
      body: { status },
    });
  }

  uploadImage(id, file) {
    const formData = new FormData();
    formData.set("file", file);
    return this.apiClient.request(`/products/${id}/image`, {
      method: "POST",
      formData,
    });
  }
}
