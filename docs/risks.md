# Riesgos y pendientes externos

| Riesgo | Impacto | Medida |
| --- | --- | --- |
| Falta rúbrica de Canvas y semana 6 | Algún criterio puede no estar cubierto | Revisar cuando se facilite y actualizar backlog y pruebas. |
| Cliente Google OAuth aún en modo de prueba | Solo usuarios de prueba pueden entrar con Google | Crear cliente web, añadir usuario de prueba y verificar en navegador. |
| Contraseña de PostgreSQL compartida en la conversación | Queda en el historial del chat | Cambiarla en Supabase tras validar la conexión y actualizar el `.env` local; no copiarla a Git. |
| Espacio del disco de Docker en C: | Puede impedir reconstrucciones | La demo alojada usa solo dos contenedores. Vigilar espacio y conservar los volúmenes anteriores. |
| Pago y productos de demostración | No representan cobro ni inventario comercial | Mostrar claramente el carácter simulado. |

No se borran volúmenes de proyectos ajenos. La pila autohospedada anterior puede detenerse sin `-v` tras completar la migración.
