import { createClient } from "@supabase/supabase-js";
import { api } from "../api/client.js";
import { toast } from "../components/toast.js";
import { appState } from "../state/appState.js";
import { go } from "../utils/navigation.js";

export async function initializeAuth() {
  let response;
  try {
    response = await fetch("/api/config");
  } catch {
    throw new Error(
      "No se pudo conectar con Qatu. Revisa que la API esté encendida.",
    );
  }
  if (!response.ok)
    throw new Error("No se pudo obtener la configuración de Qatu.");
  const config = await response.json();
  if (!config.publishableKey)
    throw new Error("Falta configurar la clave pública de Supabase.");
  appState.client = createClient(config.supabaseUrl, config.publishableKey, {
    auth: {
      persistSession: true,
      detectSessionInUrl: true,
      autoRefreshToken: true,
    },
  });
  const { data } = await appState.client.auth.getSession();
  appState.session = data.session;
  if (data.session) await refreshProfile();
  appState.client.auth.onAuthStateChange((_event, session) => {
    appState.session = session;
    if (!session) {
      appState.profile = null;
      appState.changed();
      return;
    }
    window.setTimeout(
      () => refreshProfile().catch((error) => toast(error.message, "error")),
      0,
    );
  });
  appState.changed();
}

export async function refreshProfile() {
  if (!appState.session) {
    appState.profile = null;
    appState.changed();
    return;
  }
  appState.profile = await api("/me");
  appState.changed();
}

export function requireLogin() {
  if (appState.session) return true;
  toast("Inicia sesión para continuar.", "error");
  go("/acceso");
  return false;
}
