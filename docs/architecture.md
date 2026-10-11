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
  Endpoints[Endpoints REST] --> UseCases[Casos de uso por acción]
  Hub[ChatHub] --> Conversations[AuthorizeConversationUseCase]
  UseCases --> Policies[Políticas de acceso]
  Conversations --> Policies
  UseCases --> Ports[Puertos de persistencia pequeños]
  Policies --> Ports
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

El dominio mantiene cada entidad y enumeración en su propio archivo y no depende de frameworks. La capa de aplicación organiza comandos, consultas y casos de uso explícitos por funcionalidad: usuarios, catálogo, vendedores, productos, pedidos, conversaciones, reseñas y reportes. Cada comando, consulta, modelo, regla y caso de uso tiene su propio archivo; `ProductData`, `ProductRules` y `CreateProductUseCase`, por ejemplo, ya no comparten una unidad de código. Las reglas de acceso compartidas viven en políticas pequeñas. La persistencia mantiene interfaces segregadas con `IReadRepository`, `IWriteRepository`, `IUnitOfWork` e `IStockRepository`. Infraestructura implementa esos puertos con EF Core y Supabase Storage. La API separa contratos, mapeadores, serialización, manejo de errores y registro de dependencias; cada endpoint recibe únicamente el caso de uso que ejecuta.

La SPA tiene una operación de aplicación por archivo dentro de `usecases`; ya no existen servicios de funcionalidades que agrupen acciones no relacionadas. Las vistas son funciones puras y delegan secciones complejas a componentes pequeños por pantalla. Los ViewModels coordinan estado y presentación, mientras los controladores de UI encapsulan enlaces DOM complejos como la administración de productos del vendedor. `createApplication.js` y `createUseCases.js` componen dependencias; `ViewModelFactory` resuelve rutas y crea ViewModels; `main.js` se limita al arranque y ciclo de vida. Los estilos se importan por área y mantienen el orden de la cascada original: base, pantallas y ajustes de identidad/responsividad. `npm run format` y `npm run format:check` aplican y verifican el formato de JavaScript y CSS.
