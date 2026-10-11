import assert from "node:assert/strict";
import test from "node:test";
import { CreateProductUseCase } from "../../src/usecases/seller/CreateProductUseCase.js";
import { UpdateProductUseCase } from "../../src/usecases/seller/UpdateProductUseCase.js";

test("product use cases use distinct create and update operations", async () => {
  const calls = [];
  const apiClient = {
    request(path, options) {
      calls.push({ path, options });
      return {};
    },
  };

  await new CreateProductUseCase(apiClient).execute({ name: "Café" });
  await new UpdateProductUseCase(apiClient).execute("product-id", {
    name: "Café especial",
  });

  assert.equal(calls[0].path, "/products");
  assert.equal(calls[0].options.method, "POST");
  assert.equal(calls[1].path, "/products/product-id");
  assert.equal(calls[1].options.method, "PUT");
});
