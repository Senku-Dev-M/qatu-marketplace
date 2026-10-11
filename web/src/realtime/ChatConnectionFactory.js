import * as signalR from "@microsoft/signalr";

export class ChatConnectionFactory {
  constructor(getAccessToken) {
    this.getAccessToken = getAccessToken;
  }

  create() {
    return new signalR.HubConnectionBuilder()
      .withUrl("/hubs/chat", { accessTokenFactory: this.getAccessToken })
      .withAutomaticReconnect()
      .build();
  }
}
