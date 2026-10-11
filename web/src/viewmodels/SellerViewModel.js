import { BaseViewModel } from "./BaseViewModel.js";
import { bindProductActions } from "../controllers/seller/bindProductActions.js";
import { bindProductEditor } from "../controllers/seller/bindProductEditor.js";
import { bindSellerApplication } from "../controllers/seller/bindSellerApplication.js";
import { sellerView } from "../views/sellerView.js";

export class SellerViewModel extends BaseViewModel {
  constructor({
    applyAsSeller,
    loadSellerWorkspace,
    createProduct,
    updateProduct,
    changeProductStatus,
    uploadProductImage,
    sessionGuard,
    state,
    notifier,
  }) {
    super();
    this.applyAsSeller = applyAsSeller;
    this.loadSellerWorkspace = loadSellerWorkspace;
    this.createProduct = createProduct;
    this.updateProduct = updateProduct;
    this.changeProductStatus = changeProductStatus;
    this.uploadProductImage = uploadProductImage;
    this.sessionGuard = sessionGuard;
    this.notifier = notifier;
    this.state = {
      loading: true,
      shop: null,
      products: [],
      editing: null,
      profile: state.profile,
    };
  }
  async load() {
    if (!this.sessionGuard.requireLogin()) return;
    if (this.state.profile?.isSeller) {
      Object.assign(this.state, await this.loadSellerWorkspace.execute());
    }
    this.state.loading = false;
    this.render();
  }
  template() {
    return sellerView(this.state);
  }
  bind() {
    bindSellerApplication({
      root: this.el,
      applyAsSeller: this.applyAsSeller,
      notifier: this.notifier,
    });
    bindProductEditor({
      root: this.el,
      state: this.state,
      createProduct: this.createProduct,
      updateProduct: this.updateProduct,
      notifier: this.notifier,
      render: () => this.render(),
      reload: () => this.load(),
    });
    bindProductActions({
      root: this.el,
      changeProductStatus: this.changeProductStatus,
      uploadProductImage: this.uploadProductImage,
      notifier: this.notifier,
      reload: () => this.load(),
    });
  }
}
