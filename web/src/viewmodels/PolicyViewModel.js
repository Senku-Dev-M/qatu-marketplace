import { BaseViewModel } from "./BaseViewModel.js";
import { privacyView } from "../views/privacyView.js";
import { termsView } from "../views/termsView.js";

export class PolicyViewModel extends BaseViewModel {
  constructor(kind) {
    super();
    this.kind = kind;
  }
  template() {
    return this.kind === "privacy" ? privacyView() : termsView();
  }
}
