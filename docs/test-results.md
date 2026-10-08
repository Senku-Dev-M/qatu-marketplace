# Evidencia de pruebas

## Supabase Cloud — 29-09-2026

Qatu utiliza Auth, PostgreSQL y Storage administrados por Supabase Cloud. Localmente se ejecutan únicamente la SPA y la API en `http://localhost:8081`. El archivo con secretos se mantiene ignorado por Git.

| Comprobación | Resultado observado |
| --- | --- |
| `docker compose ... -f infra/hosted/compose.yml config --quiet` | Configuración válida. |
| `docker compose ... up -d --build` | API y web saludables; la API aplicó la migración del esquema `qatu` en PostgreSQL administrado. |
| `GET /health` y `/api/config` | `healthy`, URL administrada y clave publishable; la clave secret no sale al navegador. |
| Storage | Bucket `qatu-products` privado y lectura autorizada desde la API. |
| `dotnet test Qatu.slnx -c Release` | 4/4 pruebas pasaron. |
| `npm run build` | Correcto. |
| `python scripts/seed_demo.py accounts` y `content` | Cuentas de demostración y catálogo visibles. |
| `python scripts/test_live.py` | Salud, roles, búsqueda, imagen privada, compra, stock, calificación, historial, reporte y CSV correctos. |
| `python scripts/test_signalr.py` | Entrega a comprador y vendedor; administrador ajeno rechazado. |

La prueba de imagen en borrador confirmó que la API pública responde 404 para un producto no publicado y que el archivo existe en el bucket privado. Google OAuth y el recorrido autenticado se validaron posteriormente con una cuenta de prueba.

## Rediseño, catálogo y OAuth — 29-09-2026

- La migración `ProductDetails` se aplicó correctamente en PostgreSQL administrado.
- El catálogo mostró 21 productos publicados con sus imágenes en Supabase Storage.
- La portada, tarjetas, detalle de producto, acceso por correo y Google OAuth se revisaron en navegador.
- La vista móvil de 390 px no presentó desbordamiento horizontal.
- La entrega SignalR llegó al comprador y vendedor, mientras el administrador ajeno fue rechazado.

## Refactor SOLID, Clean Architecture y MVVM — 08-10-2026

- `dotnet format Qatu.slnx --no-restore`: correcto.
- `dotnet build Qatu.slnx --no-restore`: correcto, 0 advertencias y 0 errores.
- `dotnet test Qatu.slnx --no-build`: 4/4 pruebas correctas con los servicios separados.
- `npm test`: 4/4 pruebas correctas para el cliente HTTP y los servicios de catálogo, vendedor y pedidos.
- `npm run build` y `npm run format:check`: correctos.
- Navegador real con la API refactorizada: catálogo con 21 productos, detalle de producto y pantalla de acceso cargaron correctamente; no se registraron errores ni advertencias en consola.
- No se repitieron recorridos autenticados que crean pedidos, reseñas o mensajes para evitar modificar los datos administrados durante una verificación de arquitectura.

## Limpieza de infraestructura — 08-10-2026

- Se retiraron los 63 archivos de la distribución local de Supabase.
- Los scripts de seed e integración ahora utilizan exclusivamente `infra/hosted/.env` y Supabase Cloud.
- Docker Compose conserva únicamente los servicios de Qatu: API y web.

## Casos de uso explícitos — 08-10-2026

- La capa de aplicación reemplazó los servicios agrupados por comandos, consultas y una clase de caso de uso por acción.
- Los endpoints y el hub dependen directamente del caso de uso que ejecutan; las reglas compartidas de autorización se concentran en políticas reutilizables.
- `dotnet format Qatu.slnx --no-restore`: correcto.
- `dotnet build Qatu.slnx --no-restore`: correcto, 0 advertencias y 0 errores.
- `dotnet test Qatu.slnx --no-build`: 4/4 pruebas correctas usando directamente los nuevos casos de uso.
