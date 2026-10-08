import { BaseViewModel } from "./BaseViewModel.js";
import * as signalR from "@microsoft/signalr";
import { api } from "../api/client.js";
import { appState } from "../state/appState.js";
import { requireLogin } from "../auth/session.js";
import { toast } from "../components/toast.js";
import { inboxView } from "../views/inboxView.js";

export class InboxViewModel extends BaseViewModel {
  constructor(id) {
    super();
    this.state = {
      loading: true,
      conversations: [],
      activeId: id,
      messages: [],
    };
    this.connection = null;
  }
  async load() {
    if (!requireLogin()) return;
    this.state.conversations = await api("/conversations");
    if (this.state.activeId) await this.open(this.state.activeId);
    this.state.loading = false;
    this.render();
  }
  async open(id) {
    this.state.activeId = id;
    this.state.messages = await api(`/conversations/${id}/messages`);
    if (this.connection) await this.connection.stop();
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/chat", {
        accessTokenFactory: () => appState.session?.access_token || "",
      })
      .withAutomaticReconnect()
      .build();
    this.connection.on("message", (message) => {
      if (message.conversationId !== this.state.activeId) return;
      if (!this.state.messages.some((existing) => existing.id === message.id))
        this.state.messages.push(message);
      this.render();
      this.el.querySelector("#messages")?.scrollTo(0, 999999);
    });
    this.connection.onreconnected(() =>
      this.connection
        .invoke("Join", this.state.activeId)
        .catch((error) => toast(error.message, "error")),
    );
    await this.connection.start();
    await this.connection.invoke("Join", id);
    this.render();
  }
  template() {
    return inboxView(this.state);
  }
  bind() {
    this.el.querySelectorAll("[data-conversation]").forEach((button) =>
      button.addEventListener("click", async () => {
        try {
          await this.open(button.dataset.conversation);
          window.history.replaceState(
            null,
            "",
            `#/mensajes/${button.dataset.conversation}`,
          );
        } catch (error) {
          toast(error.message, "error");
        }
      }),
    );
    this.el
      .querySelector("#message-form")
      ?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const input = event.target.elements.text;
        const text = input.value.trim();
        if (!text) return;
        input.value = "";
        try {
          await api(`/conversations/${this.state.activeId}/messages`, {
            method: "POST",
            body: { text },
          });
        } catch (error) {
          input.value = text;
          toast(error.message, "error");
        }
      });
  }
  destroy() {
    this.connection?.stop();
  }
}
