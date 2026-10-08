import { esc } from "../utils/format.js";

export function empty(title, detail, action = "") {
  return `<div class="empty"><span class="empty-llama"><img src="/art/llama-mascot.png" alt="" /></span><h3>${esc(title)}</h3><p>${esc(detail)}</p>${action}</div>`;
}
