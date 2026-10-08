import { BaseViewModel } from "./BaseViewModel.js";
import { authView } from "../views/authView.js";

export class AuthViewModel extends BaseViewModel {
  constructor({ auth, notifier, navigate }) {
    super();
    this.auth = auth;
    this.notifier = notifier;
    this.navigate = navigate;
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
              ? await this.auth.signUp(values)
              : await this.auth.signIn({
                  email: values.email,
                  password: values.password,
                });
          if (result.error) throw result.error;
          if (result.data.session) {
            this.notifier("Bienvenido a Qatu.");
            this.navigate("/");
          } else this.notifier("Revisa tu correo para confirmar la cuenta.");
        } catch (error) {
          this.state.error = error.message;
          this.render();
        }
      });
    this.el
      .querySelector("#google-login")
      .addEventListener("click", async () => {
        const result = await this.auth.signInWithGoogle();
        if (result.error) {
          this.state.error = result.error.message;
          this.render();
        }
      });
  }
}
