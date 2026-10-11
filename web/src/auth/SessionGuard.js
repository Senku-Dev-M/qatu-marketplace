export class SessionGuard {
  constructor({ state, notifier, navigate }) {
    this.state = state;
    this.notifier = notifier;
    this.navigate = navigate;
  }

  requireLogin() {
    if (this.state.session) return true;
    this.notifier("Inicia sesión para continuar.", "error");
    this.navigate("/acceso");
    return false;
  }
}
