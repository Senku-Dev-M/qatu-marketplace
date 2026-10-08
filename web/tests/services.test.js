import assert from "node:assert/strict";
import test from "node:test";
import { ApiClient } from "../src/api/ApiClient.js";
import { CatalogService } from "../src/services/CatalogService.js";
import { OrderService } from "../src/services/OrderService.js";
import { SellerService } from "../src/services/SellerService.js";

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

test("CatalogService owns catalog query construction", async () => {
  const calls = [];
  const service = new CatalogService({
    request(path) {
      calls.push(path);
      return [];
    },
  });

  await service.search("café", "Bebidas");
  await service.search("", "Todo");

  assert.deepEqual(calls, [
    "/catalog?q=caf%C3%A9&category=Bebidas",
    "/catalog?",
  ]);
});

test("SellerService selects create and update operations", async () => {
  const calls = [];
  const service = new SellerService({
    request(path, options) {
      calls.push({ path, options });
      return {};
    },
  });

  await service.save(null, { name: "Café" });
  await service.save("product-id", { name: "Café especial" });

  assert.equal(calls[0].path, "/products");
  assert.equal(calls[0].options.method, "POST");
  assert.equal(calls[1].path, "/products/product-id");
  assert.equal(calls[1].options.method, "PUT");
});

test("OrderService converts review stars at its boundary", async () => {
  let call;
  const service = new OrderService({
    request(path, options) {
      call = { path, options };
      return {};
    },
  });

  await service.review("order-id", {
    kind: "Product",
    stars: "5",
    comment: "Excelente",
  });

  assert.equal(call.path, "/reviews");
  assert.deepEqual(call.options.body, {
    orderId: "order-id",
    kind: "Product",
    stars: 5,
    comment: "Excelente",
  });
});
