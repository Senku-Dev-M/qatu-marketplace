export class ApiClient {
  constructor(getAccessToken, baseUrl = "/api") {
    this.getAccessToken = getAccessToken;
    this.baseUrl = baseUrl;
  }

  async request(path, { method = "GET", body, formData } = {}) {
    const response = await this.fetch(path, {
      method,
      headers: body === undefined ? {} : { "Content-Type": "application/json" },
      body: formData || (body === undefined ? undefined : JSON.stringify(body)),
    });
    return response.status === 204 ? null : response.json();
  }

  async download(path) {
    return (await this.fetch(path)).blob();
  }

  async fetch(path, options = {}) {
    const headers = { ...options.headers };
    const accessToken = this.getAccessToken();
    if (accessToken) headers.Authorization = `Bearer ${accessToken}`;
    let response;
    try {
      response = await fetch(`${this.baseUrl}${path}`, { ...options, headers });
    } catch {
      throw new Error(
        "No se pudo conectar con Qatu. Revisa tu conexión e inténtalo otra vez.",
      );
    }
    if (response.ok) return response;
    let data = null;
    try {
      data = await response.json();
    } catch {
      // The API can return an empty body for infrastructure failures.
    }
    throw new Error(data?.detail || data?.title || `Error ${response.status}`);
  }
}
