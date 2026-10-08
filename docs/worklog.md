# Registro de trabajo

## Semana 1 — alcance y fundamentos

Se leyó el material disponible y se fijó un marketplace individual en tres fases. Se creó el backlog y se eligió SPA MVVM con JavaScript, API .NET 10 y servicios administrados en Supabase Cloud. Se definieron roles, estados, diagramas y criterios de aceptación.

## Semanas 2–3 — infraestructura e identidad

Se preparó Compose para la SPA y la API, secretos locales ignorados por Git, migración del esquema `qatu` en Supabase Cloud, autenticación por correo, validación de token y perfil del comprador. El catálogo público y la búsqueda quedaron integrados.

## Semanas 4–5 — recorrido intermedio

Se añadieron solicitudes de vendedor, aprobación administrativa, tienda, productos, imágenes, publicación, compra directa y pago simulado en BOB. La prueba intermedia usa un administrador, un vendedor y un comprador.

## Semanas 6–10 — interacción y cierre

Se añadió chat con SignalR e historial, calificaciones vinculadas al pedido pagado, reportes y CSV, configuración para Google OAuth, pruebas unitarias e integración local, OpenAPI, salud y pipeline GitLab. El resultado exacto de las pruebas se registra en `test-results.md`.

Esta secuencia es la trazabilidad de la implementación del proyecto, no una afirmación de trabajo ocurrido durante diez semanas reales. No hubo revisión de otro integrante; la revisión propia se consigna en `review.md`.

## Integración con Supabase Cloud

Por decisión del propietario, Qatu utiliza un proyecto administrado en Supabase Cloud y el proyecto Google Cloud `qatu-sd4`. SPA y API se ejecutan mediante `infra/hosted/compose.yml`, se configuró el Session pooler y se ejecutó la migración EF. Se crearon cuentas y productos de demo en la base administrada. Las pruebas de integración se registran en `test-results.md`.

## Refinamiento visual y autenticación — 29-09-2026

Se sustituyeron la escena y la marca dibujadas en CSS/SVG por recursos generados para Qatu, se integró el ícono oficial de Google y se añadieron 21 fotografías verificadas desde fichas de producto. El seed sube las imágenes a Storage y la SPA usa las copias locales para el catálogo. Se habilitó Google en Supabase y se recorrió el acceso con la cuenta propietaria hasta una sesión iniciada. Para publicar OAuth a cualquier cuenta, Google solicita un dominio verificable con páginas públicas de inicio y privacidad; se prepararon las páginas de privacidad y condiciones dentro de la SPA.

## Organización del código — 29-09-2026

Se separaron entidades, enumeraciones, contratos y rutas en archivos individuales. La API consume `IMarketplaceService`, el servicio usa `IAppRepository` e `IImageStore`, y las implementaciones quedaron agrupadas por persistencia, repositorio, almacenamiento, autenticación y hub. La SPA separa cada vista, ViewModel y componente; sus estilos conservan el mismo orden de cascada. El README documenta el flujo de autenticación con Google y correo en PlantUML. Las verificaciones concretas constan en `test-results.md`.

## Refactor SOLID, Clean Architecture y MVVM — 08-10-2026

El servicio general de marketplace se reemplazó por contratos y servicios por capacidad. La persistencia se segregó en puertos de lectura, escritura, transacción y stock, y cada endpoint depende solo del caso de uso que necesita. En la SPA, el cliente HTTP, autenticación y servicios de funcionalidades se inyectan desde un punto de composición; una fábrica crea los ViewModels y las vistas dejaron de importar estado global. Se conservaron las rutas y el comportamiento visible.
