export class CatalogService {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  search(query, category) {
    const params = new URLSearchParams();
    if (query) params.set("q", query);
    if (category && category !== "Todo") params.set("category", category);
    return this.apiClient.request(`/catalog?${params}`);
  }

  product(id) {
    return this.apiClient.request(`/products/${id}`);
  }
}
