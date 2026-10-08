# Decisiones de arquitectura

| ID | Decisión | Motivo y consecuencia |
| --- | --- | --- |
| ADR-01 | SPA en JavaScript y Vite, MVVM por pantalla | Expone los fundamentos web y la separación entre vista, estado y acciones sin framework. Las rutas se resuelven en cliente. |
| ADR-02 | API .NET 10 con Domain, Application, Infrastructure y API | Las reglas y permisos quedan en casos de uso y son probables sin HTTP. EF Core migra únicamente el esquema `qatu`. |
| ADR-03 | Supabase Cloud en proyecto propio | Auth, PostgreSQL y Storage se administran en Supabase Cloud; Docker Compose solo levanta la SPA y la API. Se reduce la carga de contenedores locales. La API conecta por Session pooler con TLS y conserva SignalR para el chat. |
| ADR-04 | Token validado con Supabase Auth en cada petición | Revocación y estado de sesión quedan bajo Auth. Aumenta un salto interno por petición; se evita confiar en un token sin validación. |
| ADR-05 | Pago simulado con resultado explícito | Demuestra estados, stock y calificaciones sin recibir tarjeta ni afirmar que existe cobro. |
| ADR-06 | Roles y propiedad en la API | Supabase autentica, Qatu autoriza. El ID administrativo se configura fuera de Git antes de iniciar la API. |
| ADR-07 | Stock con actualización condicional transaccional | Dos compras simultáneas no pueden llevar las existencias por debajo de cero. |

## Seguridad y operación

`infra/hosted/.env` contiene secretos y está ignorado. La clave secret de Supabase solo se entrega a la API; `/api/config` expone únicamente la clave publishable. Las fotos se validan por tipo y tamaño; el bucket es privado y la API sirve las imágenes. CORS permite los orígenes locales de la demostración. La conexión PostgreSQL exige TLS. Para exposición pública futura harán falta HTTPS para la SPA, correo SMTP y secretos gestionados.
