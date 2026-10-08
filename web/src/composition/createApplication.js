import { ApiClient } from "../api/ApiClient.js";
import { AuthService } from "../auth/AuthService.js";
import { toast } from "../components/toast.js";
import { AdminService } from "../services/AdminService.js";
import { CatalogService } from "../services/CatalogService.js";
import { ConversationService } from "../services/ConversationService.js";
import { OrderService } from "../services/OrderService.js";
import { SellerService } from "../services/SellerService.js";
import { appState } from "../state/appState.js";
import { go } from "../utils/navigation.js";

export function createApplication() {
  const accessToken = () => appState.session?.access_token || "";
  const apiClient = new ApiClient(accessToken);
  return {
    state: appState,
    notifier: toast,
    navigate: go,
    auth: new AuthService({
      state: appState,
      apiClient,
      notifier: toast,
      navigate: go,
    }),
    catalog: new CatalogService(apiClient),
    seller: new SellerService(apiClient),
    orders: new OrderService(apiClient),
    conversations: new ConversationService(apiClient, accessToken),
    admin: new AdminService(apiClient),
  };
}
