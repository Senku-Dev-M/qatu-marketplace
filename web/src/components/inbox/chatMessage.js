import { esc } from "../../utils/format.js";

export function chatMessage(message, currentUserId) {
  const mine = message.senderId === currentUserId ? "mine" : "";
  const time = new Date(message.sentAt).toLocaleTimeString("es-BO", {
    hour: "2-digit",
    minute: "2-digit",
  });
  return `<div class="message ${mine}"><p>${esc(message.text)}</p><small>${time}</small></div>`;
}
