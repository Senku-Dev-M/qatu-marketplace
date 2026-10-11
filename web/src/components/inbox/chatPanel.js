import { chatMessage } from "./chatMessage.js";

export function chatPanel(state) {
  if (!state.activeId)
    return '<div class="chat-panel"><div class="chat-placeholder">✳<p>Selecciona una conversación</p></div></div>';

  const messages = state.messages
    .map((message) => chatMessage(message, state.currentUserId))
    .join("");
  return `<div class="chat-panel">
    <div class="chat-heading">
      <span class="conversation-avatar">✳</span>
      <div><strong>Consulta sobre producto</strong><small>Solo tú y la otra persona ven este chat</small></div>
    </div>
    <div class="messages" id="messages">${messages}</div>
    <form id="message-form" class="message-form">
      <input name="text" required maxlength="1000" placeholder="Escribe un mensaje..." aria-label="Mensaje">
      <button class="button" type="submit">Enviar ↗</button>
    </form>
  </div>`;
}
