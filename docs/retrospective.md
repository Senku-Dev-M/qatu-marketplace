# Retrospectiva

## Funcionó

La separación por capas permitió probar permisos y reglas sin contenedores, mientras la integración local comprobó las uniones entre Auth, API, base de datos y Storage. El resultado simulado del pago permitió demostrar inventario y autorización de calificaciones sin manejar datos financieros. La navegación MVVM quedó pequeña y fácil de explicar en una presentación individual.

## Cambios durante la implementación

La prueba en vivo reveló que la conversión de un estado aceptaba valores numéricos fuera del enum. Se validan ahora los estados definidos. La primera subida también reveló que esta versión de Supabase Storage comunica un bucket ausente como `400 NoSuchBucket`; el cliente admite esa respuesta. Ambas correcciones se verificaron de nuevo en contenedores.

## Próximo ajuste

Cuando estén disponibles la semana 6 y la rúbrica de Canvas, comparar sus criterios con `docs/backlog.md`. Configurar y probar un cliente OAuth de Google. Mover el disco de Docker a D: desde Docker Desktop y volver a comprobar Qatu y los contenedores anteriores. Si el proyecto deja de ser una demo local, añadir TLS, correo SMTP, gestión de secretos y políticas de despliegue.
