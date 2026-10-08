"""Run integration checks against Qatu backed by Supabase Cloud."""

from __future__ import annotations

import json
import base64
import urllib.error
import urllib.parse
import urllib.request
import uuid

import seed_demo
from seed_demo import ACCOUNTS

BASE = ""


def call(
    path: str, bearer: str | None = None, method: str = "GET", body: dict | None = None
):
    headers = {"Content-Type": "application/json"}
    if bearer:
        headers["Authorization"] = f"Bearer {bearer}"
    data = json.dumps(body).encode() if body is not None else None
    request = urllib.request.Request(
        BASE + path, data=data, headers=headers, method=method
    )
    try:
        with urllib.request.urlopen(request, timeout=30) as response:
            raw = response.read()
            if "json" in response.headers.get("Content-Type", ""):
                return response.status, json.loads(raw)
            return response.status, raw
    except urllib.error.HTTPError as error:
        return error.code, error.read()


def expect(
    expected: int,
    path: str,
    bearer: str | None = None,
    method: str = "GET",
    body: dict | None = None,
):
    status, data = call(path, bearer, method, body)
    assert (
        status == expected
    ), f"{method} {path}: expected {expected}, got {status}; {str(data)[:250]}"
    return data


def main() -> None:
    global BASE
    env = seed_demo.configure_cloud()
    BASE = seed_demo.APP
    admin, seller, buyer = [seed_demo.token(email, env) for email in ACCOUNTS]
    expect(200, "/health")
    expect(401, "/api/me")
    buyer_profile = expect(200, "/api/me", buyer)
    seller_profile = expect(200, "/api/me", seller)
    assert not buyer_profile["isAdmin"] and not buyer_profile["isSeller"]
    assert seller_profile["isSeller"]
    expect(403, "/api/admin/report", buyer)
    expect(403, "/api/seller-applications", buyer)

    catalog = expect(200, "/api/catalog")
    assert len(catalog) >= 6
    item = catalog[0]
    assert item["status"] == "Published"
    assert any(
        p["id"] == item["id"]
        for p in expect(200, "/api/catalog?q=" + urllib.parse.quote(item["name"]))
    )
    expect(200, f"/api/products/{item['id']}")
    expect(
        403, f"/api/products/{item['id']}/status", buyer, "POST", {"status": "Hidden"}
    )

    mine = expect(200, "/api/products/mine", seller)
    assert mine and all(p["sellerId"] == seller_profile["id"] for p in mine)
    expect(400, f"/api/products/{item['id']}/status", seller, "POST", {"status": "999"})
    expect(
        403,
        "/api/products",
        buyer,
        "POST",
        {"name": "X", "description": "", "category": "Arte", "priceBob": 1, "stock": 1},
    )

    png = base64.b64decode(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/lWQAAAAASUVORK5CYII="
    )
    image_product = next(
        (p for p in mine if p["name"] == "Prueba privada de imagen"), None
    ) or expect(
        200,
        "/api/products",
        seller,
        "POST",
        {
            "name": "Prueba privada de imagen",
            "description": "Borrador para verificar Storage.",
            "category": "Arte",
            "priceBob": 1,
            "stock": 1,
        },
    )
    boundary = uuid.uuid4().hex
    multipart = (
        (
            f'--{boundary}\r\nContent-Disposition: form-data; name="file"; filename="test.png"\r\n'
            "Content-Type: image/png\r\n\r\n"
        ).encode()
        + png
        + f"\r\n--{boundary}--\r\n".encode()
    )
    upload = urllib.request.Request(
        BASE + f"/api/products/{image_product['id']}/image",
        data=multipart,
        method="POST",
        headers={
            "Authorization": f"Bearer {seller}",
            "Content-Type": f"multipart/form-data; boundary={boundary}",
        },
    )
    with urllib.request.urlopen(upload, timeout=30) as response:
        assert response.status == 200
        image_product = json.load(response)
    status, _ = call(f"/api/products/{image_product['id']}/image")
    assert status == 404  # Draft images remain inaccessible through the public API.
    storage_url = f"{seed_demo.AUTH}/storage/v1/object/authenticated/qatu-products/{image_product['imagePath']}"
    stored = urllib.request.Request(
        storage_url,
        headers={
            "apikey": env["QATU_SECRET_KEY"],
            "User-Agent": "Qatu-Server-Setup/1.0",
        },
    )
    with urllib.request.urlopen(stored, timeout=30) as response:
        assert response.read() == png

    failed = expect(
        200,
        "/api/orders",
        buyer,
        "POST",
        {"productId": item["id"], "quantity": 1, "paymentSucceeds": False},
    )
    assert failed["status"] == "Failed"
    expect(
        403,
        "/api/reviews",
        buyer,
        "POST",
        {
            "orderId": failed["id"],
            "kind": "Product",
            "stars": 5,
            "comment": "No procede",
        },
    )
    before = expect(200, f"/api/products/{item['id']}")["product"]["stock"]
    assert before >= 1
    paid = expect(
        200,
        "/api/orders",
        buyer,
        "POST",
        {"productId": item["id"], "quantity": 1, "paymentSucceeds": True},
    )
    assert paid["status"] == "Paid" and paid["unitPriceBob"] == item["priceBob"]
    after = expect(200, f"/api/products/{item['id']}")["product"]["stock"]
    assert after == before - 1
    expect(
        409,
        "/api/orders",
        buyer,
        "POST",
        {"productId": item["id"], "quantity": 99, "paymentSucceeds": True},
    )
    review = expect(
        200,
        "/api/reviews",
        buyer,
        "POST",
        {
            "orderId": paid["id"],
            "kind": "Product",
            "stars": 5,
            "comment": "Prueba de integración",
        },
    )
    assert review["stars"] == 5
    expect(
        409,
        "/api/reviews",
        buyer,
        "POST",
        {"orderId": paid["id"], "kind": "Product", "stars": 5, "comment": "Duplicado"},
    )

    conversation = expect(
        200, "/api/conversations", buyer, "POST", {"productId": item["id"]}
    )
    message = expect(
        200,
        f"/api/conversations/{conversation['id']}/messages",
        buyer,
        "POST",
        {"text": "Mensaje de prueba"},
    )
    history = expect(200, f"/api/conversations/{conversation['id']}/messages", seller)
    assert any(m["id"] == message["id"] for m in history)
    expect(403, f"/api/conversations/{conversation['id']}/messages", admin)

    report = expect(200, "/api/admin/report", admin)
    assert report["paidOrders"] >= 1 and report["salesBob"] > 0
    status, csv = call("/api/admin/report.csv", admin)
    assert status == 200 and b"fecha,pedidos,ventas_bob" in csv
    print(
        "Integración: salud, correo, roles, catálogo, imagen, compra, stock, calificación, chat privado, reporte y CSV: OK"
    )


if __name__ == "__main__":
    main()
