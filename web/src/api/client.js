import { appState } from "../state/appState.js";

export async function api(path, { method = "GET", body, formData } = {}) {
  const headers = {};
  if (appState.session?.access_token)
    headers.Authorization = `Bearer ${appState.session.access_token}`;
  if (body !== undefined) headers["Content-Type"] = "application/json";
  let response;
  try {
    response = await fetch(`/api${path}`, {
      method,
      headers,
      body: formData || (body === undefined ? undefined : JSON.stringify(body)),
    });
  } catch {
    throw new Error(
      "No se pudo conectar con Qatu. Revisa tu conexión e inténtalo otra vez.",
    );
  }
  if (!response.ok) {
    let data = null;
    try {
      data = await response.json();
    } catch {
      /* empty error body */
    }
    throw new Error(data?.detail || data?.title || `Error ${response.status}`);
  }
  return response.status === 204 ? null : response.json();
}
