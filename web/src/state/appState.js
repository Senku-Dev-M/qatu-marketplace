export const appState = {
  client: null,
  session: null,
  profile: null,
  listeners: new Set(),
  subscribe(fn) {
    this.listeners.add(fn);
    return () => this.listeners.delete(fn);
  },
  changed() {
    for (const fn of this.listeners) fn();
  },
};
