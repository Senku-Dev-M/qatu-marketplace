"""Create repeatable local demo accounts and catalog through public APIs."""

from __future__ import annotations

import argparse
import json
import secrets
import urllib.error
import urllib.request
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ENV = ROOT / "infra" / "hosted" / ".env"
AUTH = ""
APP = ""
ACCOUNTS = ("admin@qatu.local", "vendedor@qatu.local", "comprador@qatu.local")


def environment() -> dict[str, str]:
    return dict(
        line.split("=", 1)
        for line in ENV.read_text(encoding="utf-8").splitlines()
        if line and not line.startswith("#") and "=" in line
    )


def configure_cloud() -> dict[str, str]:
    global AUTH, APP
    cloud_env = environment()
    AUTH = cloud_env["QATU_SUPABASE_URL"]
    APP = f"http://localhost:{cloud_env.get('QATU_WEB_PORT', '8081')}"
    return cloud_env


def request(
    url: str,
    method: str = "GET",
    body: object | None = None,
    token: str | None = None,
    apikey: str | None = None,
) -> object:
    headers = {
        "Content-Type": "application/json",
        "User-Agent": "Qatu-Server-Setup/1.0",
    }
    if apikey:
        headers["apikey"] = apikey
    if token:
        headers["Authorization"] = f"Bearer {token}"
    payload = None if body is None else json.dumps(body).encode()
    req = urllib.request.Request(url, data=payload, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req, timeout=30) as response:
            data = response.read()
            return json.loads(data) if data else None
    except urllib.error.HTTPError as error:
        detail = error.read().decode(errors="replace")
        raise RuntimeError(
            f"{method} {url}: HTTP {error.code}: {detail[:250]}"
        ) from error


def upload_product_image(product_id: str, filename: str, seller_token: str) -> None:
    image = (ROOT / "web" / "public" / "products" / filename).read_bytes()
    boundary = "qatu-" + secrets.token_hex(12)
    payload = (
        (
            f'--{boundary}\r\nContent-Disposition: form-data; name="file"; filename="{filename}"\r\n'
            "Content-Type: image/jpeg\r\n\r\n"
        ).encode()
        + image
        + f"\r\n--{boundary}--\r\n".encode()
    )
    req = urllib.request.Request(
        f"{APP}/api/products/{product_id}/image",
        data=payload,
        method="POST",
        headers={
            "Authorization": f"Bearer {seller_token}",
            "Content-Type": f"multipart/form-data; boundary={boundary}",
            "User-Agent": "Qatu-Server-Setup/1.0",
        },
    )
    try:
        with urllib.request.urlopen(req, timeout=45):
            pass
    except urllib.error.HTTPError as error:
        detail = error.read().decode(errors="replace")
        raise RuntimeError(
            f"Upload image for {product_id}: HTTP {error.code}: {detail[:250]}"
        ) from error


def create_accounts(env: dict[str, str]) -> None:
    service = env.get("QATU_SECRET_KEY") or env["SERVICE_ROLE_KEY"]
    legacy_token = service if not service.startswith("sb_secret_") else None
    existing = request(
        f"{AUTH}/auth/v1/admin/users?per_page=1000", token=legacy_token, apikey=service
    )
    users = {user["email"]: user for user in existing.get("users", [])}
    for email in ACCOUNTS:
        if email not in users:
            users[email] = request(
                f"{AUTH}/auth/v1/admin/users",
                "POST",
                {
                    "email": email,
                    "password": env["QATU_DEMO_PASSWORD"],
                    "email_confirm": True,
                },
                legacy_token,
                service,
            )
    admin_id = users[ACCOUNTS[0]]["id"]
    lines = ENV.read_text(encoding="utf-8").splitlines()
    lines = [
        (
            f"QATU_ADMIN_USER_ID={admin_id}"
            if line.startswith("QATU_ADMIN_USER_ID=")
            else line
        )
        for line in lines
    ]
    ENV.write_text("\n".join(lines) + "\n", encoding="utf-8", newline="\n")
    print(
        "Cuentas de administrador, vendedor y comprador listas; ID administrativo configurado."
    )


def token(email: str, env: dict[str, str]) -> str:
    result = request(
        f"{AUTH}/auth/v1/token?grant_type=password",
        "POST",
        {"email": email, "password": env["QATU_DEMO_PASSWORD"]},
        apikey=env.get("QATU_PUBLISHABLE_KEY") or env["ANON_KEY"],
    )
    return result["access_token"]


def content(env: dict[str, str], refresh_images: bool = False) -> None:
    admin, seller, buyer = (token(email, env) for email in ACCOUNTS)
    profiles = [request(f"{APP}/api/me", token=item) for item in (admin, seller, buyer)]
    seller_id = profiles[1]["id"]
    if not profiles[1]["isSeller"]:
        pending = [
            item
            for item in request(f"{APP}/api/seller-applications", token=admin)
            if item["applicantId"] == seller_id and item["status"] == "Pending"
        ]
        application = (
            pending[0]
            if pending
            else request(
                f"{APP}/api/seller-applications",
                "POST",
                {
                    "shopName": "Katu de la Esquina",
                    "description": "Bebidas y productos cotidianos para el barrio.",
                },
                seller,
            )
        )
        request(
            f"{APP}/api/seller-applications/{application['id']}/decision",
            "POST",
            {"approve": True},
            admin,
        )
    existing = request(f"{APP}/api/products/mine", token=seller)
    legacy_names = {
        "Jarrón Tierra",
        "Cuaderno de Oficio",
        "Bolso Lino Natural",
        "Café de Altura",
        "Lámpara Nido",
        "Soporte Madera",
    }
    for product in existing:
        if product["name"] in legacy_names and product["status"] == "Published":
            request(
                f"{APP}/api/products/{product['id']}/status",
                "POST",
                {"status": "Hidden"},
                seller,
            )
    catalog = json.loads(
        (ROOT / "scripts" / "seed_catalog.json").read_text(encoding="utf-8")
    )
    for item in catalog:
        found = next(
            (
                p
                for p in existing
                if p["name"] == item["name"]
                and p["presentation"] == item["presentation"]
            ),
            None,
        )
        body = dict(item)
        image_file = body.pop("imageFile", None)
        if found:
            body["stock"] = found["stock"]  # Preserve purchases when the seed is rerun.
            if any(
                found[key] != body[key]
                for key in (
                    "brand",
                    "presentation",
                    "unit",
                    "description",
                    "category",
                    "priceBob",
                )
            ):
                request(f"{APP}/api/products/{found['id']}", "PUT", body, seller)
            product = found
        else:
            product = request(f"{APP}/api/products", "POST", body, seller)
        if image_file and (refresh_images or not product.get("imagePath")):
            upload_product_image(product["id"], image_file, seller)
        if product["status"] != "Published":
            request(
                f"{APP}/api/products/{product['id']}/status",
                "POST",
                {"status": "Published"},
                seller,
            )
    print(
        "Catálogo de demostración listo; credenciales en la variable QATU_DEMO_PASSWORD del .env ignorado por Git."
    )


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("stage", choices=("accounts", "content"))
    parser.add_argument(
        "--refresh-images",
        action="store_true",
        help="Replace already-uploaded seed product photos",
    )
    args = parser.parse_args()
    stage = args.stage
    env = configure_cloud()
    if "QATU_DEMO_PASSWORD" not in env:
        with ENV.open("a", encoding="utf-8") as output:
            output.write(f"QATU_DEMO_PASSWORD={secrets.token_urlsafe(18)}\n")
        env = environment()
    create_accounts(env) if stage == "accounts" else content(env, args.refresh_images)
