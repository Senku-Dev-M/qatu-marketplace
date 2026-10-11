import { empty } from "../empty.js";

function conversationItem(conversation, activeId) {
  const selected = activeId === conversation.id ? "selected" : "";
  return `<button type="button" class="conversation-item ${selected}" data-conversation="${conversation.id}">
    <span class="conversation-avatar">✳</span>
    <span><strong>Producto ${conversation.productId.slice(0, 8)}</strong><small>Conversación privada</small></span>
    <span>↗</span>
  </button>`;
}

export function conversationList(conversations, activeId) {
  return `<aside class="conversation-list">
    ${
      conversations.length
        ? conversations
            .map((conversation) => conversationItem(conversation, activeId))
            .join("")
        : empty(
            "Bandeja vacía",
            "Consulta un producto para comenzar a conversar.",
          )
    }
  </aside>`;
}
