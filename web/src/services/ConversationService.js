import * as signalR from "@microsoft/signalr";

export class ConversationService {
  constructor(apiClient, getAccessToken) {
    this.apiClient = apiClient;
    this.getAccessToken = getAccessToken;
  }

  list() {
    return this.apiClient.request("/conversations");
  }

  messages(id) {
    return this.apiClient.request(`/conversations/${id}/messages`);
  }

  send(id, text) {
    return this.apiClient.request(`/conversations/${id}/messages`, {
      method: "POST",
      body: { text },
    });
  }

  open(productId) {
    return this.apiClient.request("/conversations", {
      method: "POST",
      body: { productId },
    });
  }

  createConnection() {
    return new signalR.HubConnectionBuilder()
      .withUrl("/hubs/chat", { accessTokenFactory: this.getAccessToken })
      .withAutomaticReconnect()
      .build();
  }
}
