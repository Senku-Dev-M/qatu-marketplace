import assert from "node:assert/strict";
import test from "node:test";
import { ApiClient } from "../../src/api/ApiClient.js";

test("ApiClient adds the current access token and serializes JSON", async () => {
  const originalFetch = globalThis.fetch;
  let request;
  globalThis.fetch = async (url, options) => {
    request = { url, options };
    return new Response(JSON.stringify({ ok: true }), {
      status: 200,
      headers: { "Content-Type": "application/json" },
    });
  };

  try {
    const client = new ApiClient(() => "access-token", "/api");
    const result = await client.request("/orders", {
      method: "POST",
      body: { quantity: 2 },
    });

    assert.deepEqual(result, { ok: true });
    assert.equal(request.url, "/api/orders");
    assert.equal(request.options.headers.Authorization, "Bearer access-token");
    assert.equal(request.options.headers["Content-Type"], "application/json");
    assert.equal(request.options.body, JSON.stringify({ quantity: 2 }));
  } finally {
    globalThis.fetch = originalFetch;
  }
});
