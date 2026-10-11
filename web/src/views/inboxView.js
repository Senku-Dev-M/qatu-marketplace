import { chatPanel } from "../components/inbox/chatPanel.js";
import { conversationList } from "../components/inbox/conversationList.js";

export function inboxView(state) {
  return `<section class="wrap inner-section">
    <p class="eyebrow">CONVERSACIONES</p>
    <h1>Mensajes<span class="period">.</span></h1>
    <p class="section-lead">Conversaciones privadas sobre productos del mercado.</p>
    <div class="inbox-layout">
      ${conversationList(state.conversations, state.activeId)}
      ${chatPanel(state)}
    </div>
  </section>`;
}
