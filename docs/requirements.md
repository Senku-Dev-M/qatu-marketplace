# Requisitos y aceptación

## Actores

Visitante: explora y busca. Comprador: cuenta por defecto, compra, conversa y califica después de pagar. Vendedor: conserva facultades de comprador y administra únicamente su tienda y productos. Administrador: decide solicitudes y consulta indicadores y CSV. Ningún registro público puede otorgar el rol administrador.

## Historias verificables

| ID | Historia | Criterio de aceptación |
| --- | --- | --- |
| R1 | Como visitante, quiero explorar el catálogo | Solo se muestran productos publicados; búsqueda y categoría reducen resultados; diseño usable en móvil. |
| R2 | Como persona, quiero registrarme e iniciar sesión | Email y clave funcionan; una sesión válida crea perfil de comprador; Google usa Supabase cuando se configuran credenciales. |
| R3 | Como comprador, quiero solicitar una tienda | Solo hay una solicitud pendiente; un administrador la aprueba o rechaza; aprobar crea tienda y habilita rol vendedor. |
| R4 | Como vendedor, quiero publicar artículos | Puedo crear, editar, adjuntar JPG/PNG/WebP de hasta 5 MB y cambiar entre borrador, publicado y oculto únicamente en mis productos. |
| R5 | Como comprador, quiero comprar un producto | La cantidad es de 1 a 99, el precio queda fijado en el pedido, el pago simulado aprobado descuenta stock y uno fallido no lo descuenta. No se captura tarjeta. |
| R6 | Como comprador, quiero conversar con el vendedor | Conversación y mensajes quedan guardados; SignalR entrega cambios en vivo solo a participantes. |
| R7 | Como comprador, quiero calificar | Solo se acepta una calificación por tipo y pedido pagado propio; estrellas de 1 a 5. |
| R8 | Como administrador, quiero ver actividad | Indicadores de usuarios, vendedores, productos, pedidos pagados y monto en BOB; CSV diario descargable. |

## Fuera de alcance

Carrito, pasarela real, cobro o datos de tarjeta, entregas y logística. La calificación se vincula a la compra pagada, sin validación de recepción física.

## Definición de terminado

Código construye, pruebas de reglas e integración pasan, contenedores necesarios están saludables, se abre la aplicación y se recorren los roles con dos sesiones. Cada resultado se consigna con fecha y limitaciones en `test-results.md`.
