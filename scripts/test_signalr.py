"""Check live SignalR delivery to buyer and seller, with admin excluded."""

from __future__ import annotations

import json
import urllib.request
from contextlib import ExitStack

from websockets.sync.client import connect

import seed_demo
import test_live
from seed_demo import ACCOUNTS
from test_live import expect

RS = "\x1e"
BASE = ""


def socket(bearer: str, stack: ExitStack):
    request = urllib.request.Request(
        BASE + "/hubs/chat/negotiate?negotiateVersion=1",
        data=b"",
        method="POST",
        headers={"Authorization": f"Bearer {bearer}"},
    )
    with urllib.request.urlopen(request, timeout=20) as response:
        connection_token = json.load(response)["connectionToken"]
    ws = stack.enter_context(
        connect(
            f"{BASE.replace('http:', 'ws:')}/hubs/chat?id={connection_token}",
            additional_headers={"Authorization": f"Bearer {bearer}"},
            open_timeout=20,
        )
    )
    ws.send('{"protocol":"json","version":1}' + RS)
    assert ws.recv(timeout=20) == "{}" + RS
    return ws


def join(ws, conversation: str, invocation: str):
    ws.send(
        json.dumps(
            {
                "type": 1,
                "target": "Join",
                "arguments": [conversation],
                "invocationId": invocation,
            }
        )
        + RS
    )
    return json.loads(ws.recv(timeout=20).rstrip(RS))


def main() -> None:
    global BASE
    env = seed_demo.configure_cloud()
    BASE = seed_demo.APP
    test_live.BASE = BASE
    admin, seller, buyer = [seed_demo.token(email, env) for email in ACCOUNTS]
    product = expect(200, "/api/catalog")[0]
    conversation = expect(
        200, "/api/conversations", buyer, "POST", {"productId": product["id"]}
    )
    cid = conversation["id"]
    with ExitStack() as stack:
        b, s, a = (socket(auth, stack) for auth in (buyer, seller, admin))
        assert join(b, cid, "buyer")["type"] == 3
        assert join(s, cid, "seller")["type"] == 3
        denial = join(a, cid, "admin")
        assert denial["type"] == 3 and "error" in denial
        message = expect(
            200,
            f"/api/conversations/{cid}/messages",
            buyer,
            "POST",
            {"text": "Entrega SignalR verificada"},
        )
        for ws in (b, s):
            event = json.loads(ws.recv(timeout=20).rstrip(RS))
            assert event["type"] == 1 and event["target"] == "message"
            assert event["arguments"][0]["id"] == message["id"]
        try:
            unexpected = a.recv(timeout=2)
            raise AssertionError(
                f"Administrador recibió un evento privado: {unexpected}"
            )
        except TimeoutError:
            pass
    print(
        "SignalR: entrega a comprador y vendedor en dos conexiones; administrador excluido: OK"
    )


if __name__ == "__main__":
    main()
