import { BaseViewModel } from "./BaseViewModel.js";
import { inboxView } from "../views/inboxView.js";

export class InboxViewModel extends BaseViewModel {
  constructor(
    {
      listConversations,
      getConversationMessages,
      sendConversationMessage,
      chatConnectionFactory,
      sessionGuard,
      notifier,
      state,
    },
    id,
  ) {
    super();
    this.listConversations = listConversations;
    this.getConversationMessages = getConversationMessages;
    this.sendConversationMessage = sendConversationMessage;
    this.chatConnectionFactory = chatConnectionFactory;
    this.sessionGuard = sessionGuard;
    this.notifier = notifier;
    this.state = {
      loading: true,
      conversations: [],
      activeId: id,
      messages: [],
      currentUserId: state.profile?.id ?? null,
    };
    this.connection = null;
  }
  async load() {
    if (!this.sessionGuard.requireLogin()) return;
    this.state.conversations = await this.listConversations.execute();
    if (this.state.activeId) await this.open(this.state.activeId);
    this.state.loading = false;
    this.render();
  }
  async open(id) {
    this.state.activeId = id;
    this.state.messages = await this.getConversationMessages.execute(id);
    if (this.connection) await this.connection.stop();
    this.connection = this.chatConnectionFactory.create();
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
        .catch((error) => this.notifier(error.message, "error")),
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
          this.notifier(error.message, "error");
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
          await this.sendConversationMessage.execute(this.state.activeId, text);
        } catch (error) {
          input.value = text;
          this.notifier(error.message, "error");
        }
      });
  }
  destroy() {
    this.connection?.stop();
  }
}
