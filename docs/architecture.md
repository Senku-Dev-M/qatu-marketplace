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
  Endpoints[Endpoints REST] --> Contract[IMarketplaceService]
  Hub[ChatHub] --> Contract
  Contract --> Market[MarketplaceService]
  Market --> Store[IAppRepository]
  Market --> Images[IImageStore]
  Store --> EF[EF Core]
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

El dominio mantiene cada entidad y enumeración en su propio archivo. La capa de aplicación declara `IMarketplaceService` y `IAppRepository`, e implementa `MarketplaceService` por capacidades mediante clases parciales (`Profiles`, `Catalog`, `Sellers`, `Products`, `Orders`, `Conversations`, `Reviews` y `Reports`). `Repositories`, `Storage`, `Models` y `Errors` contienen sus contratos y tipos individuales. Infraestructura agrupa persistencia, repositorio EF y Storage. La API registra rutas por capacidad en `Endpoints`, con `Contracts`, `Authentication` y `Hubs` separados; `Program.cs` contiene la composición, middleware y arranque. El README incluye el flujo PlantUML de Supabase Auth con Google y correo.

La SPA tiene un archivo por vista, componente y ViewModel. `main.js` conecta las rutas, mientras que `api`, `auth`, `state`, `data` y `utils` agrupan responsabilidades compartidas. Los estilos se importan por área y mantienen el orden de la cascada original: base, pantallas y ajustes de identidad/responsividad. `npm run format` y `npm run format:check` aplican y verifican el formato de JavaScript y CSS.
