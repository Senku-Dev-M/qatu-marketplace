# Contrato API

OpenAPI generado al ejecutar la API: `/openapi/v1.json`. Base: `/api`. Las rutas privadas requieren `Authorization: Bearer <token Supabase>`. Los errores siguen Problem Details con códigos 400, 401, 403, 404 o 409. Importes en BOB.

| Método | Ruta | Acceso | Uso |
| --- | --- | --- | --- |
| GET | `/config` | Público | URL de Supabase y clave pública |
| GET | `/catalog?q=&category=` | Público | Búsqueda, categoría y productos publicados |
| GET | `/products/{id}` | Público | Producto y calificaciones |
| GET | `/products/{id}/image` | Público | Imagen desde Storage |
| GET | `/sellers/{id}/reviews` | Público | Calificaciones de vendedor |
| GET | `/me` | Cuenta | Perfil y rol |
| POST | `/seller-applications` | Comprador | Solicitar tienda |
| GET | `/seller-applications` | Administrador | Solicitudes |
| POST | `/seller-applications/{id}/decision` | Administrador | `{ "approve": true }` |
| GET | `/shops/mine` | Vendedor | Tienda propia |
| GET | `/products/mine` | Vendedor | Productos propios, incluidos borradores |
| POST, PUT | `/products`, `/products/{id}` | Vendedor propietario | Nombre, marca, presentación, unidad, descripción, categoría, precio, stock |
| POST | `/products/{id}/status` | Vendedor propietario | `Draft`, `Published`, `Hidden` |
| POST | `/products/{id}/image` | Vendedor propietario | Multipart `file`, hasta 5 MB |
| POST | `/orders` | Comprador | Producto, cantidad y resultado simulado |
| GET | `/orders` | Participante | Pedidos propios |
| POST, GET | `/conversations`, `/conversations` | Cuenta | Abrir y listar conversaciones |
| GET, POST | `/conversations/{id}/messages` | Participante | Historial y envío |
| POST | `/reviews` | Comprador con pedido pagado | Tipo, estrellas y comentario |
| GET | `/admin/report`, `/admin/report.csv` | Administrador | Indicadores y CSV |

Hub: `/hubs/chat`, método `JoinConversation(conversationId)`, evento `message`. El servidor comprueba pertenencia antes de añadir la conexión al grupo. Endpoint de salud: `/health`.
