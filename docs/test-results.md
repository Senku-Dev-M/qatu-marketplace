# Evidencia de pruebas locales

Fecha: 29 de septiembre de 2026. Entorno: Windows, Docker Desktop, .NET 10, Node 24; aplicación en `http://localhost:8080`. Los resultados son locales. El archivo `.gitlab-ci.yml` está versionado, pero no se ha ejecutado en GitLab.

| Comprobación | Resultado observado |
| --- | --- |
| `dotnet build Qatu.slnx --no-restore -c Release` | Correcto, 0 advertencias y 0 errores. |
| `dotnet test Qatu.slnx --no-restore` | 4/4 pruebas unitarias pasaron: aprobación y propiedad, pago fallido, compra y stock, privacidad de historial. |
| `npm run build` en `web` | Correcto. |
| `python scripts/test_live.py` | Correcto: salud, autenticación por correo, roles, catálogo/búsqueda, imagen en Storage, compra, stock, calificación, mensajes privados, reporte y CSV. |
| `python scripts/test_signalr.py` | Correcto: mensaje entregado por SignalR a comprador y vendedor; el administrador ajeno no pudo unirse ni recibió evento. |
| Navegador real | SPA abierta, catálogo y detalle recorridos; se creó cuenta local, se completó pago simulado y se envió una reseña desde la interfaz. |
| Vista móvil 390 × 844 | Ancho interior 390 px y ancho del documento 375 px; sin desbordamiento horizontal. |
| Contenedores | Auth, PostgreSQL, Storage, gateway, API y web iniciaron saludables. |

La prueba de integración descubrió dos fallos y ambos se corrigieron antes de repetirla: `Enum.TryParse` aceptaba un número como estado de producto, y Storage respondió `400 NoSuchBucket` durante la primera subida, mientras el cliente esperaba `404`. Tras reconstruir la API, la prueba completa pasó.

**Aún sin verificación externa:** Google OAuth requiere un cliente de Google y su secreto; el flujo está cableado y deshabilitado por defecto. El traslado del disco de Docker a D: requiere la interfaz de Docker Desktop, que no está disponible en el control de aplicaciones de esta sesión. La semana 6 y la rúbrica de Canvas no estuvieron disponibles. La prueba de error de red no se automatizó; la SPA muestra mensajes en español cuando falla `fetch`.

## Migración a Supabase alojado — 29 de septiembre de 2026

Proyecto Supabase Qatu (`zrrvjlnfvpcbvspfpbts`) en plan Free, región `us-west-2`. SPA/API local en `http://localhost:8081`; la pila autohospedada anterior se detuvo con `down` sin borrar volúmenes y la alojada siguió saludable. El archivo local de secretos está ignorado por Git.

| Comprobación | Resultado observado |
| --- | --- |
| `docker compose ... -f infra/hosted/compose.yml config --quiet` | Configuración válida. |
| `docker compose ... up -d --build` | API y web saludables. API aplicó la migración del esquema `qatu` en PostgreSQL alojado. |
| `GET /health` y `/api/config` | `healthy`, URL alojada y clave publishable; la clave secret no sale al navegador. |
| Storage | Bucket `qatu-products` creado privado y lectura autorizada con clave secret. |
| `dotnet test Qatu.slnx -c Release` | 4/4 pruebas pasaron. |
| `npm run build` | Correcto. |
| `python scripts/seed_demo.py accounts --hosted` y `content --hosted` | Tres cuentas y seis productos visibles en el catálogo. |
| `python scripts/test_live.py --hosted` | Salud, roles, búsqueda, imagen privada, compra, stock, calificación, historial, reporte y CSV correctos. |
| `python scripts/test_signalr.py --hosted` | Entrega a dos participantes; administrador ajeno rechazado. |
| Chrome | Qatu abre, muestra seis productos y permite navegar el catálogo. |

Al abrir en Chrome apareció `400 Request Header Or Cookie Too Large` por cookies preexistentes de `localhost`; se amplió el búfer de cabeceras de Nginx, se reconstruyó la web y el catálogo abrió correctamente. La prueba inicial de imagen en borrador mostró que la API pública responde 404 para un producto no publicado; la prueba se ajustó para comprobar por separado que el archivo existe en Storage privado. Google OAuth y el recorrido en navegador autenticado se registrarán tras crear el cliente y añadir un usuario de prueba.

# Validación del rediseño y catálogo (29-09-2026)

- Compilación Vite de producción: correcta.
- `dotnet test Qatu.slnx -c Release`: 4/4 pruebas correctas.
- Migración `ProductDetails` aplicada al arrancar la API alojada; API y web saludables en Docker Compose.
- `python scripts/seed_demo.py content --hosted`: 21 productos publicados, seis productos antiguos ocultos y tienda de demostración renombrada a Katu de la Esquina.
- `python scripts/test_live.py --hosted`: salud, correo, roles, catálogo, imagen, compra, stock, calificación, chat privado, reporte y CSV correctos.
- `python scripts/test_signalr.py --hosted`: entrega al comprador y vendedor en dos conexiones; administrador excluido.
- Inspección visual en Chrome: portada, tarjetas, ficha de Coca-Cola Original y presentación de 500 ml / Bs 6 / botella; navegación al catálogo corregida. Se comprobó la vista móvil y se ajustó el ancho mínimo del documento.

Los precios y el stock son datos de demostración. En esa validación Google OAuth aún no estaba configurado; su prueba posterior se registra abajo.

# Recursos visuales y Google OAuth (29-09-2026)

- `python scripts/prepare_product_images.py`: 21 fotografías JPG verificadas y enlazadas al catálogo. Las fuentes quedan en `scripts/product_image_sources.json`.
- `python scripts/seed_demo.py content --hosted` y `--refresh-images`: 21 productos publicados con imagen almacenada; `GET /api/catalog` devolvió 21 productos y 21 con `hasImage=true`. La imagen de Coca-Cola Original respondió `200 image/jpeg`.
- `npm run build`: compilación de producción correcta tras integrar el recurso oficial de Google, ilustraciones generadas y fotos locales. `python -m py_compile` para los scripts editados y `git diff --check` pasaron.
- Supabase Google habilitado: `/auth/v1/authorize?provider=google` respondió `302` hacia Google; desapareció el error `Unsupported provider: provider is not enabled`.
- Chrome: selección de cuenta, consentimiento de nombre/correo y retorno a Qatu correctos. La SPA mostró una sesión iniciada y el catálogo. No se incluyeron tokens ni secretos en el repositorio ni en este documento.
- Chrome: portada, tarjetas con fotos y acceso revisados visualmente en escritorio; portada y tarjetas revisadas también en un viewport de 390 px. Se restauró el tamaño normal al finalizar.
- Google Cloud sigue en modo **Prueba**. El acceso público para cualquier cuenta requiere completar la información de marca con una web y política de privacidad en un dominio verificable y publicar la app.

# Refactor de estructura (29-09-2026)

- `dotnet test Qatu.slnx -c Release --no-restore`: 4/4 pruebas correctas tras separar entidades, servicio, contratos y rutas.
- `npm run format:check` y `npm run build`: correctos tras dividir vistas, ViewModels, componentes y estilos.
- Docker Compose reconstruido: API y web saludables; `GET /health` respondió 200 y el catálogo devolvió 21 productos.
- Rutas autenticadas con cuentas de demostración: `/api/products/mine` para vendedor y `/api/admin/report` para administrador respondieron 200.
- Playwright con Chrome: 21 tarjetas visibles, pantalla de acceso y botón Google presentes; sin errores de JavaScript ni desbordamiento horizontal en 390 px.
