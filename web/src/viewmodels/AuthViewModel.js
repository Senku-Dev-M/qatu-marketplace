import { BaseViewModel } from "./BaseViewModel.js";
import { appState } from "../state/appState.js";
import { go } from "../utils/navigation.js";
import { authView } from "../views/authView.js";
import { toast } from "../components/toast.js";

export class AuthViewModel extends BaseViewModel {
  constructor() {
    super();
    this.state = { loading: false, mode: "login", error: "" };
  }
  template() {
    return authView(this.state);
  }
  bind() {
    this.el.querySelector("#auth-toggle").addEventListener("click", () => {
      this.state.mode = this.state.mode === "login" ? "register" : "login";
      this.state.error = "";
      this.render();
    });
    this.el
      .querySelector("#auth-form")
      .addEventListener("submit", async (event) => {
        event.preventDefault();
        const values = Object.fromEntries(new FormData(event.target));
        try {
          const result =
            this.state.mode === "register"
              ? await appState.client.auth.signUp({
                  email: values.email,
                  password: values.password,
                  options: { data: { full_name: values.name } },
                })
              : await appState.client.auth.signInWithPassword({
                  email: values.email,
                  password: values.password,
                });
          if (result.error) throw result.error;
          if (result.data.session) {
            toast("Bienvenido a Qatu.");
            go("/");
          } else toast("Revisa tu correo para confirmar la cuenta.");
        } catch (error) {
          this.state.error = error.message;
          this.render();
        }
      });
    this.el
      .querySelector("#google-login")
      .addEventListener("click", async () => {
        const result = await appState.client.auth.signInWithOAuth({
          provider: "google",
          options: { redirectTo: location.origin },
        });
        if (result.error) {
          this.state.error = result.error.message;
          this.render();
        }
      });
  }
}
