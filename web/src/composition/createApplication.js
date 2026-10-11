import { ApiClient } from "../api/ApiClient.js";
import { AuthService } from "../auth/AuthService.js";
import { SessionGuard } from "../auth/SessionGuard.js";
import { toast } from "../components/toast.js";
import { ChatConnectionFactory } from "../realtime/ChatConnectionFactory.js";
import { appState } from "../state/appState.js";
import { go } from "../utils/navigation.js";
import { createUseCases } from "./createUseCases.js";

export function createApplication() {
  const accessToken = () => appState.session?.access_token || "";
  const apiClient = new ApiClient(accessToken);
  const sharedDependencies = {
    state: appState,
    apiClient,
    notifier: toast,
    navigate: go,
  };
  return {
    state: appState,
    notifier: toast,
    navigate: go,
    auth: new AuthService(sharedDependencies),
    sessionGuard: new SessionGuard(sharedDependencies),
    chatConnectionFactory: new ChatConnectionFactory(accessToken),
    ...createUseCases(apiClient),
  };
}
