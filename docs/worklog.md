# Registro de trabajo

## Semana 1 — alcance y fundamentos

Se leyó el material disponible y se fijó un marketplace individual en tres fases. Se creó el backlog y se eligió SPA MVVM con JavaScript, API .NET 10 y Supabase local. Se definieron roles, estados, diagramas y criterios de aceptación.

## Semanas 2–3 — infraestructura e identidad

Se preparó Compose con los servicios necesarios, secretos locales ignorados por Git, migración del esquema `qatu`, autenticación por correo, validación de token y perfil del comprador. El catálogo público y la búsqueda quedaron integrados.

## Semanas 4–5 — recorrido intermedio

Se añadieron solicitudes de vendedor, aprobación administrativa, tienda, productos, imágenes, publicación, compra directa y pago simulado en BOB. La prueba intermedia usa un administrador, un vendedor y un comprador.

## Semanas 6–10 — interacción y cierre

Se añadió chat con SignalR e historial, calificaciones vinculadas al pedido pagado, reportes y CSV, configuración para Google OAuth, pruebas unitarias e integración local, OpenAPI, salud y pipeline GitLab. El resultado exacto de las pruebas se registra en `test-results.md`.

Esta secuencia es la trazabilidad de la implementación del proyecto, no una afirmación de trabajo ocurrido durante diez semanas reales. No hubo revisión de otro integrante; la revisión propia se consigna en `review.md`.

## Ajuste de infraestructura — Supabase alojado

Por decisión del propietario, se creó una organización Free y un proyecto Qatu alojado en Supabase, además del proyecto Google Cloud `qatu-sd4`. Se separaron SPA/API en `infra/hosted/compose.yml`, se configuró el Session pooler y se ejecutó la migración EF. Se crearon cuentas y productos de demo en la nueva base. Las pruebas de integración alojada se registran en `test-results.md`.

## Refinamiento visual y autenticación — 29-09-2026

Se sustituyeron la escena y la marca dibujadas en CSS/SVG por recursos generados para Qatu, se integró el ícono oficial de Google y se añadieron 21 fotografías verificadas desde fichas de producto. El seed sube las imágenes a Storage y la SPA usa las copias locales para el catálogo. Se habilitó Google en Supabase y se recorrió el acceso con la cuenta propietaria hasta una sesión iniciada. Para publicar OAuth a cualquier cuenta, Google solicita un dominio verificable con páginas públicas de inicio y privacidad; se prepararon las páginas de privacidad y condiciones dentro de la SPA.

## Organización del código — 29-09-2026

Se separaron entidades, enumeraciones, contratos y rutas en archivos individuales. La API consume `IMarketplaceService`, el servicio usa `IAppRepository` e `IImageStore`, y las implementaciones quedaron agrupadas por persistencia, repositorio, almacenamiento, autenticación y hub. La SPA separa cada vista, ViewModel y componente; sus estilos conservan el mismo orden de cascada. El README documenta el flujo de autenticación con Google y correo en PlantUML. Las verificaciones concretas constan en `test-results.md`.
