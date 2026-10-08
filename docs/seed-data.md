# Catálogo de demostración

`scripts/seed_catalog.json` define 21 artículos de minimarket y mercado boliviano: 15 bebidas y 6 artículos de snacks, despensa y limpieza. Cada registro incluye nombre comercial, marca, categoría, presentación, unidad de venta, precio en BOB, stock, descripción y fotografía asociada. Las fotos se obtuvieron de fichas públicas de producto; sus páginas y URLs exactas constan en `scripts/product_image_sources.json`. `scripts/seed_demo.py content --hosted` las carga mediante la API en el bucket privado. Un vendedor puede subir o reemplazar sus propias fotos mediante la carga de imágenes existente.

Los importes son aproximaciones para la **demo académica**, no cotizaciones ni inventario en tiempo real. Se contrastaron presentaciones y precios con comercios bolivianos:

- [Multicenter: Coca-Cola y otras bebidas](https://www.multicenter.com/coca-cola) para Coca-Cola, Sprite, Fanta, Vital, Del Valle, Powerade y Monster.
- [Amarket: Pepsi 500 ml](https://amarket.com.bo/collections/gaseosas/products/909851?_fid=13db36368&_pos=16&_ss=c), [aguas](https://amarket.com.bo/collections/agua) y [bebidas](https://amarket.com.bo/collections/bebidas-y-licores?grid_list=&page=6).
- [Amarket: arroz Ovando](https://amarket.com.bo/products/7771430370010), [tallarín Lazzaroni](https://amarket.com.bo/products/7772500000356) y [limpiador Ola](https://amarket.com.bo/products/7779970830537).
- [Compro: Cremosita Mabel's](https://compro.bo/products/galletas-cremosita-chocolate-mabels-pack-6ux38g) y catálogos de Oreo/Breick.

El seed es idempotente por nombre y presentación. Al repetirlo, actualiza atributos comerciales sin restablecer las unidades vendidas. Los artículos antiguos se ocultan en lugar de borrarse, para conservar los pedidos y reseñas de pruebas previas. El esquema suma `Brand`, `Presentation` y `Unit` mediante la migración `ProductDetails`.
