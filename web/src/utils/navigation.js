export function go(path) {
  window.location.hash = `#${path}`;
}
export function currentPath() {
  return window.location.hash.slice(1) || "/";
}
