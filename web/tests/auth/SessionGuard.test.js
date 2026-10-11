import assert from "node:assert/strict";
import test from "node:test";
import { SessionGuard } from "../../src/auth/SessionGuard.js";

test("SessionGuard redirects anonymous users", () => {
  const notifications = [];
  const routes = [];
  const guard = new SessionGuard({
    state: { session: null },
    notifier: (...args) => notifications.push(args),
    navigate: (route) => routes.push(route),
  });

  assert.equal(guard.requireLogin(), false);
  assert.deepEqual(routes, ["/acceso"]);
  assert.deepEqual(notifications, [["Inicia sesión para continuar.", "error"]]);
});
