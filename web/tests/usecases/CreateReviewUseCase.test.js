import assert from "node:assert/strict";
import test from "node:test";
import { CreateReviewUseCase } from "../../src/usecases/reviews/CreateReviewUseCase.js";

test("CreateReviewUseCase converts stars at its API boundary", async () => {
  let call;
  const useCase = new CreateReviewUseCase({
    request(path, options) {
      call = { path, options };
      return {};
    },
  });

  await useCase.execute("order-id", {
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
