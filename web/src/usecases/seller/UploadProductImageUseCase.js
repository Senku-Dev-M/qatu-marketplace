export class UploadProductImageUseCase {
  constructor(apiClient) {
    this.apiClient = apiClient;
  }

  execute(productId, file) {
    const formData = new FormData();
    formData.set("file", file);
    return this.apiClient.request(`/products/${productId}/image`, {
      method: "POST",
      formData,
    });
  }
}
