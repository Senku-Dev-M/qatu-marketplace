import { createClient } from "@supabase/supabase-js";

export class AuthService {
  constructor({ state, apiClient, notifier, navigate }) {
    this.state = state;
    this.apiClient = apiClient;
    this.notifier = notifier;
    this.navigate = navigate;
  }

  async initialize() {
    const response = await this.apiClient.fetch("/config");
    const config = await response.json();
    if (!config.publishableKey)
      throw new Error("Falta configurar la clave pública de Supabase.");
    this.state.client = createClient(
      config.supabaseUrl,
      config.publishableKey,
      {
        auth: {
          persistSession: true,
          detectSessionInUrl: true,
          autoRefreshToken: true,
        },
      },
    );
    const { data } = await this.state.client.auth.getSession();
    this.state.session = data.session;
    if (data.session) await this.refreshProfile();
    this.state.client.auth.onAuthStateChange((_event, session) => {
      this.state.session = session;
      if (!session) {
        this.state.profile = null;
        this.state.changed();
        return;
      }
      window.setTimeout(
        () =>
          this.refreshProfile().catch((error) =>
            this.notifier(error.message, "error"),
          ),
        0,
      );
    });
    this.state.changed();
  }

  async refreshProfile() {
    this.state.profile = this.state.session
      ? await this.apiClient.request("/me")
      : null;
    this.state.changed();
  }

  async signUp(values) {
    return this.state.client.auth.signUp({
      email: values.email,
      password: values.password,
      options: { data: { full_name: values.name } },
    });
  }

  signIn(values) {
    return this.state.client.auth.signInWithPassword(values);
  }

  signInWithGoogle() {
    return this.state.client.auth.signInWithOAuth({
      provider: "google",
      options: { redirectTo: location.origin },
    });
  }

  async signOut() {
    await this.state.client.auth.signOut();
    this.notifier("Sesión cerrada.");
    this.navigate("/");
  }
}
