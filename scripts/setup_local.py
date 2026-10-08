"""Create a local Supabase .env with unique credentials. Never prints secrets."""
from __future__ import annotations

import base64
import hashlib
import hmac
import json
import secrets
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "infra" / "supabase" / ".env.example"
TARGET = ROOT / "infra" / "supabase" / ".env"


def b64(data: bytes) -> str:
    return base64.urlsafe_b64encode(data).rstrip(b"=").decode()


def jwt(role: str, secret: str) -> str:
    header = b64(json.dumps({"alg": "HS256", "typ": "JWT"}, separators=(",", ":")).encode())
    now = int(time.time())
    payload = b64(json.dumps({"role": role, "iss": "supabase", "iat": now, "exp": now + 5 * 365 * 86400}, separators=(",", ":")).encode())
    message = f"{header}.{payload}".encode()
    return f"{header}.{payload}.{b64(hmac.new(secret.encode(), message, hashlib.sha256).digest())}"


def replace_env(content: str, key: str, value: str) -> str:
    lines = content.splitlines()
    for index, line in enumerate(lines):
        if line.startswith(key + "="):
            lines[index] = f"{key}={value}"
            break
    else:
        lines.append(f"{key}={value}")
    return "\n".join(lines) + "\n"


def main() -> None:
    if TARGET.exists():
        print("Ya existe la configuración local; se conservó sin cambios.")
        return
    content = SOURCE.read_text(encoding="utf-8")
    jwt_secret = secrets.token_urlsafe(48)
    values = {
        "POSTGRES_PASSWORD": secrets.token_hex(20),
        "JWT_SECRET": jwt_secret,
        "ANON_KEY": jwt("anon", jwt_secret),
        "SERVICE_ROLE_KEY": jwt("service_role", jwt_secret),
        "DASHBOARD_USERNAME": "qatu",
        "DASHBOARD_PASSWORD": secrets.token_hex(16),
        "SECRET_KEY_BASE": secrets.token_urlsafe(64),
        "REALTIME_DB_ENC_KEY": secrets.token_hex(8),
        "VAULT_ENC_KEY": secrets.token_hex(16),
        "PG_META_CRYPTO_KEY": secrets.token_urlsafe(32),
        "LOGFLARE_PUBLIC_ACCESS_TOKEN": secrets.token_urlsafe(32),
        "LOGFLARE_PRIVATE_ACCESS_TOKEN": secrets.token_urlsafe(32),
        "S3_PROTOCOL_ACCESS_KEY_ID": secrets.token_hex(16),
        "S3_PROTOCOL_ACCESS_KEY_SECRET": secrets.token_hex(32),
        "POOLER_TENANT_ID": "qatu-local",
        "OPENAI_API_KEY": "",
        "SITE_URL": "http://localhost:8080",
        "ADDITIONAL_REDIRECT_URLS": "http://localhost:8080/**,http://localhost:5173/**",
        "ENABLE_EMAIL_AUTOCONFIRM": "true",
        "ENABLE_PHONE_SIGNUP": "false",
        "QATU_ADMIN_USER_ID": "",
        "QATU_DEMO_PASSWORD": secrets.token_urlsafe(12),
        "GOOGLE_ENABLED": "false",
        "GOOGLE_CLIENT_ID": "",
        "GOOGLE_SECRET": "",
    }
    for key, value in values.items():
        content = replace_env(content, key, value)
    TARGET.write_text(content, encoding="utf-8", newline="\n")
    print("Configuración local generada en infra/supabase/.env (ignorada por Git).")


if __name__ == "__main__":
    main()
