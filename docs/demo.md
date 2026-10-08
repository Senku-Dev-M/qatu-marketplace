# Guion de demostración

1. Comprobar `docker compose --env-file infra/hosted/.env -f infra/hosted/compose.yml ps` y `/health`. Abrir <http://localhost:8081> en escritorio y en ancho móvil.
2. Como visitante, filtrar por Bebidas, buscar «Pepsi» y abrir el detalle. Mostrar marca, presentación de 500 ml, unidad, stock y precio en Bs. La compra usa pago simulado.
3. Entrar como comprador. Solicitar tienda. Entrar como administrador y aprobar. Volver al comprador, ahora vendedor; crear producto, adjuntar imagen, publicar y comprobar que aparece en el catálogo.
4. En una segunda sesión de comprador, abrir producto, iniciar conversación y enviar un mensaje. Ver la entrega en vivo en la sesión vendedora y el historial tras recargar.
5. Comprar una unidad con resultado aprobado; mostrar que disminuye el stock. Calificar el producto. Intentar calificar el pedido fallido para enseñar la restricción.
6. Como administrador, consultar indicadores y descargar CSV. Mostrar OpenAPI, pruebas, backlog, decisiones y riesgos.
7. Probar Google con una cuenta añadida como usuario de prueba en Google Auth Platform. Registrar el resultado observado sin confundir configuración con prueba completada.

Las contraseñas de demo están en `infra/hosted/.env`, ignorado por Git. No mostrarlas en capturas públicas. Al terminar, detener los contenedores con `down` sin `-v` si se desea conservar la demo y su base alojada.
