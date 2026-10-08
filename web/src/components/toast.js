export function toast(message, tone = "success") {
  const host = document.querySelector("#toasts");
  if (!host) return;
  const item = document.createElement("div");
  item.className = `toast toast--${tone}`;
  item.textContent = message;
  host.append(item);
  window.setTimeout(() => item.remove(), 5000);
}
