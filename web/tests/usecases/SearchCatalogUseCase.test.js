import assert from "node:assert/strict";
import test from "node:test";
import { SearchCatalogUseCase } from "../../src/usecases/catalog/SearchCatalogUseCase.js";

test("SearchCatalogUseCase builds catalog filters", async () => {
  const calls = [];
  const useCase = new SearchCatalogUseCase({
    request(path) {
      calls.push(path);
      return [];
    },
  });

  await useCase.execute("café", "Bebidas");
  await useCase.execute("", "Todo");

  assert.deepEqual(calls, [
    "/catalog?q=caf%C3%A9&category=Bebidas",
    "/catalog?",
  ]);
});
