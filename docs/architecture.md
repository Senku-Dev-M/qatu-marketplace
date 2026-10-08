# Arquitectura

## C4: contexto

```mermaid
flowchart LR
  Buyer[Comprador] --> Qatu[Qatu marketplace]
  Seller[Vendedor] --> Qatu
  Admin[Administrador] --> Qatu
  Qatu --> Google[Google OAuth]
```

## C4: contenedores

```mermaid
flowchart LR
  Browser[Navegador SPA MVVM] -->|REST y SignalR| API[ASP.NET Core API]
  Browser -->|Auth| Auth[Supabase Auth]
  API -->|valida sesión| Auth
  API --> DB[(PostgreSQL, esquema qatu)]
  API --> Storage[Supabase Storage]
```

## C4: componentes de API

```mermaid
flowchart LR
  Endpoints[Endpoints REST] --> UseCases[Servicios por capacidad]
  Hub[ChatHub] --> Conversations[IConversationService]
  UseCases --> Ports[Puertos de persistencia pequeños]
  Conversations --> Ports
  UseCases --> Images[IImageStore]
  Ports --> EF[EF Core]
  Images --> Supabase[Storage API]
```

## Entidades y relaciones

```mermaid
erDiagram
  Profile ||--o{ SellerApplication : solicita
  Profile ||--o| Shop : posee
  Shop ||--o{ Product : publica
  Profile ||--o{ PurchaseOrder : compra
  Product ||--o{ PurchaseOrder : integra
  Product ||--o{ Conversation : origina
  Conversation ||--o{ ChatMessage : contiene
  PurchaseOrder ||--o{ Review : habilita
```

## Estados

```mermaid
stateDiagram-v2
  [*] --> Pending
  Pending --> Approved
  Pending --> Rejected
```

Solicitud: pendiente → aprobada o rechazada. Producto: borrador → publicado ↔ oculto, con edición posterior. Pedido: se crea pendiente y en la misma transacción termina pagado o fallido. No hay estado de envío.

## Compra y conversación

```mermaid
sequenceDiagram
  participant B as Comprador
  participant A as API
  participant D as PostgreSQL
  B->>A: POST /orders (producto, cantidad, simulación)
  A->>D: Transacción y descuento condicional de stock
  D-->>A: Pedido pagado o error de stock
  A-->>B: Estado y total BOB
  B->>A: POST /conversations/{id}/messages
  A->>D: Guardar mensaje
  A-->>B: Confirmación
  A-->>B: Evento SignalR a participantes
```

Los diagramas tienen fuente editable Mermaid en este archivo. Las seis vistas exportadas están en `docs/diagrams/*.svg`; se regeneran con `python scripts/export_diagrams.py`. Auth, PostgreSQL y Storage se alojan en Supabase; SPA y API viven en Docker local. El modelo evita acceso directo del cliente al esquema de negocio. El hub valida pertenencia antes de unir a un grupo. La API valida el token con Supabase Auth y verifica rol y propiedad de cada recurso.

## Organización del código

El dominio mantiene cada entidad y enumeración en su propio archivo y no depende de frameworks. La capa de aplicación expone contratos pequeños por caso de uso: usuarios y acceso, catálogo, vendedores, productos, pedidos, conversaciones, reseñas y reportes. También aplica segregación de interfaces a la persistencia con `IReadRepository`, `IWriteRepository`, `IUnitOfWork` e `IStockRepository`. Infraestructura implementa esos puertos con EF Core y Supabase Storage. La API registra cada contrato e implementación en el punto de composición y cada endpoint recibe solo el servicio de la capacidad que utiliza.

La SPA tiene un archivo por vista, componente y ViewModel. Las vistas son funciones puras: reciben estado y no importan estado global, navegación ni acceso HTTP. Los ViewModels coordinan cada pantalla mediante servicios por capacidad. `createApplication.js` es el único punto de composición para API, autenticación, estado, navegación y notificaciones; `ViewModelFactory` resuelve rutas y crea ViewModels; `main.js` se limita al arranque y ciclo de vida. Los estilos se importan por área y mantienen el orden de la cascada original: base, pantallas y ajustes de identidad/responsividad. `npm run format` y `npm run format:check` aplican y verifican el formato de JavaScript y CSS.
