"""Export concise SVG diagrams from the architecture model in docs/architecture.md."""
from __future__ import annotations

from pathlib import Path
from xml.etree.ElementTree import Element, SubElement, ElementTree

OUT = Path(__file__).resolve().parents[1] / "docs" / "diagrams"
OUT.mkdir(parents=True, exist_ok=True)


def svg(title: str, width=1120, height=560):
    root = Element("svg", {"xmlns": "http://www.w3.org/2000/svg", "viewBox": f"0 0 {width} {height}", "role": "img", "aria-label": title})
    SubElement(root, "rect", {"width": str(width), "height": str(height), "fill": "#f7f2e9"})
    defs = SubElement(root, "defs")
    marker = SubElement(defs, "marker", {"id": "arrow", "viewBox": "0 0 10 10", "refX": "9", "refY": "5", "markerWidth": "7", "markerHeight": "7", "orient": "auto-start-reverse"})
    SubElement(marker, "path", {"d": "M 0 0 L 10 5 L 0 10 z", "fill": "#b65e42"})
    SubElement(root, "text", {"x": "48", "y": "59", "fill": "#173d32", "font-family": "Georgia,serif", "font-size": "32", "font-weight": "bold"}).text = title
    SubElement(root, "line", {"x1": "48", "y1": "76", "x2": str(width - 48), "y2": "76", "stroke": "#d5c7b6"})
    return root


def node(root, x, y, width, title, subtitle="", color="#173d32"):
    SubElement(root, "rect", {"x": str(x), "y": str(y), "width": str(width), "height": "98", "rx": "11", "fill": color})
    SubElement(root, "text", {"x": str(x + 17), "y": str(y + 40), "fill": "#fffaf1", "font-family": "Arial,sans-serif", "font-size": "19", "font-weight": "bold"}).text = title
    if subtitle:
        SubElement(root, "text", {"x": str(x + 17), "y": str(y + 68), "fill": "#fff4de", "font-family": "Arial,sans-serif", "font-size": "13"}).text = subtitle


def edge(root, x1, y1, x2, y2, label=""):
    SubElement(root, "line", {"x1": str(x1), "y1": str(y1), "x2": str(x2), "y2": str(y2), "stroke": "#b65e42", "stroke-width": "3", "marker-end": "url(#arrow)"})
    if label:
        SubElement(root, "text", {"x": str((x1+x2)//2), "y": str((y1+y2)//2 - 8), "text-anchor": "middle", "fill": "#6b4b3f", "font-family": "Arial,sans-serif", "font-size": "13"}).text = label


def save(name, root):
    ElementTree(root).write(OUT / f"{name}.svg", encoding="utf-8", xml_declaration=True)


r = svg("C4 · Contexto")
for x, y, title, desc in [(75, 135, "Comprador", "Busca y compra"), (75, 285, "Vendedor", "Publica productos"), (75, 435, "Administrador", "Decide y reporta")]:
    node(r, x, y, 215, title, desc)
node(r, 465, 278, 250, "Qatu", "Marketplace individual", "#bd6244")
node(r, 840, 278, 220, "Google OAuth", "Acceso por Google")
for yy in (184, 334, 484): edge(r, 290, yy, 465, 327)
edge(r, 715, 327, 840, 327, "identidad")
save("contexto", r)

r = svg("C4 · Contenedores")
for x, y, w, title, desc in [(75, 160, 245, "SPA MVVM", "Navegador · Vite"), (440, 160, 245, "API .NET 10", "REST · SignalR"), (805, 160, 245, "PostgreSQL", "Supabase · esquema qatu"), (75, 380, 245, "Supabase Auth", "Correo · Google"), (805, 380, 245, "Supabase Storage", "Imágenes privadas")]:
    node(r, x, y, w, title, desc)
edge(r, 320, 209, 440, 209, "REST / hub")
edge(r, 685, 209, 805, 209, "EF Core")
edge(r, 200, 258, 200, 380, "sesión")
edge(r, 560, 258, 320, 428, "valida token")
edge(r, 685, 258, 805, 428, "archivo")
save("contenedores", r)

r = svg("C4 · Componentes de API")
for x, y, w, title, desc in [(58, 145, 210, "REST", "Endpoints"), (58, 355, 210, "ChatHub", "Grupos privados"), (385, 250, 270, "Use cases", "Comandos · consultas"), (775, 145, 270, "Repository ports", "Lectura · escritura · stock"), (775, 355, 270, "IImageStore", "Supabase Storage")]:
    node(r, x, y, w, title, desc)
edge(r, 268, 194, 385, 299)
edge(r, 268, 404, 385, 299)
edge(r, 655, 299, 775, 194)
edge(r, 655, 299, 775, 404)
save("componentes", r)

r = svg("ER · Datos de Qatu", 1220, 600)
items = [(55, 150, "Perfil", "rol y correo"), (345, 150, "Solicitud", "estado y tienda"), (635, 150, "Tienda", "propietario"), (925, 150, "Producto", "stock y precio"), (55, 395, "Pedido", "compra y pago"), (345, 395, "Calificación", "pedido pagado"), (635, 395, "Conversación", "comprador y vendedor"), (925, 395, "Mensaje", "historial")]
for x, y, title, desc in items: node(r, x, y, 220, title, desc)
for a, b in [((275,199),(345,199)),((565,199),(635,199)),((855,199),(925,199)),((165,248),(165,395)),((275,444),(345,444)),((1035,248),(745,395)),((855,444),(925,444))]: edge(r,*a,*b)
save("entidades", r)

r = svg("Estados de negocio", 1100, 570)
for y, label, values in [(145,"Solicitud",["Pendiente","Aprobada","Rechazada"]),(285,"Producto",["Borrador","Publicado","Oculto"]),(425,"Pedido",["Pendiente","Pagado","Fallido"])]:
    SubElement(r,"text",{"x":"52","y":str(y+52),"fill":"#173d32","font-family":"Arial,sans-serif","font-size":"20","font-weight":"bold"}).text=label
    for i, value in enumerate(values): node(r, 215+i*290, y, 205, value, "", "#bd6244" if i==0 else "#173d32")
    edge(r,420,y+49,505,y+49)
    edge(r,710,y+49,795,y+49)
save("estados", r)

r = svg("Secuencia · Compra y chat", 1220, 620)
actors = [(115,"Comprador"),(430,"API"),(745,"PostgreSQL"),(1030,"Vendedor")]
for x,label in actors:
    node(r,x-80,115,190,label)
    SubElement(r,"line",{"x1":str(x+15),"y1":"213","x2":str(x+15),"y2":"570","stroke":"#c9bdaf","stroke-dasharray":"6 5"})
steps=[(260,130,445,"1. Crear pedido"),(325,445,760,"2. Descontar stock"),(390,760,445,"3. Guardar pedido"),(455,130,445,"4. Enviar mensaje"),(520,445,1045,"5. Evento privado")]
for y,x1,x2,label in steps: edge(r,x1,y,x2,y,label)
save("secuencia", r)
print(f"Exportados 6 SVG en {OUT}")
