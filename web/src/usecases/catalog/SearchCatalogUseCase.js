export class SearchCatalogUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(query, category) {
    const params = new URLSearchParams();
    if (query) params.set("q", query);
    if (category && category !== "Todo") params.set("category", category);
    return this.apiClient.request(`/catalog?${params}`);
  }
}
