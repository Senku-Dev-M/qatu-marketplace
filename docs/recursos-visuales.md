# Recursos visuales de la demostración

La composición del mercado, la llamita y el distintivo de Qatu se generaron para este proyecto. Los archivos de esta entrega se conservan en `web/public/art/`. El botón de Google usa el ícono oficial para Android y web del [paquete de marca de Google](https://developers.google.com/identity/branding-guidelines).

Las 21 fotografías de productos provienen de fichas públicas de comercios bolivianos. `scripts/product_image_sources.json` registra, para cada artículo, su página de origen y URL de imagen; `scripts/prepare_product_images.py` reproduce la descarga y vincula las fotos con el catálogo semilla. Las marcas y fotografías pertenecen a sus respectivos titulares. Se incluyen para la demostración académica; antes de publicar Qatu comercialmente se necesitarían derechos de uso o fotografías propias.

La API guarda las imágenes del catálogo semilla en Storage mediante `scripts/seed_demo.py content --hosted`; la SPA conserva copias locales para mostrar los productos aun antes de esa carga. Las imágenes que suba un vendedor siguen el flujo normal de la API.
