#Buscador de Rutas del Campus

> Proyecto final – Estructuras de Datos  
> Problema 1: Buscador de rutas del campus

## Integrantes

| Nombre | Rol |
|--------|-----|
| Emily Quintero Rivera | Desarrollador / Documentador |
| Sofia Hidalgo Cordoba | Desarrollador / Documentador |

---

## Descripción

Sistema que modela un campus universitario como un **grafo no dirigido con pesos**, donde cada nodo es una ubicación (bloque, sala, cafetería, etc.) y cada arista representa una ruta peatonal con su distancia en metros.

Implementa el algoritmo de **Dijkstra** para encontrar la ruta más corta entre dos puntos y **BFS** para listar todas las ubicaciones alcanzables desde un origen.

---

## Instrucciones de ejecución

### Requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download) o superior

### Pasos

```bash
# 1. Clonar el repositorio
git clone https://github.com/tu-usuario/campus-rutas.git
cd campus-rutas

# 2. Compilar y ejecutar
cd src
dotnet run
```

El programa carga automáticamente el campus desde `data/campus.txt`. También puedes cargar cualquier archivo desde el menú.

### Formato del archivo de datos

```
# Comentarios con #
UBICACION NombreLugar
RUTA Origen;Destino;MetrosEntreEllos
```

---

## Estructura del repositorio

```
campus-rutas/
├── src/
│   ├── Grafo.cs              # Clase principal del grafo (lista de adyacencia)
│   ├── CargadorArchivo.cs    # Lectura de datos desde archivo
│   ├── Program.cs            # Menú interactivo principal
│   └── CampusRutas.csproj    # Proyecto .NET
├── docs/
│   └── documento_tecnico.md  # Documento técnico completo
├── data/
│   └── campus.txt            # Campus universitario ficticio de ejemplo
└── README.md
```

---

## Operaciones disponibles

| # | Operación | Algoritmo | Complejidad |
|---|-----------|-----------|-------------|
| 1 | Cargar desde archivo | Lectura línea a línea | O(V + E) |
| 2 | Agregar ubicación | Inserción en diccionario | O(1) |
| 3 | Eliminar ubicación | Recorrido de vecinos | O(V + E) |
| 4 | Agregar ruta | Inserción en lista de adyacencia | O(1) |
| 5 | Eliminar ruta | Eliminación en lista de adyacencia | O(1) |
| 6 | Ruta más corta | **Dijkstra** | O((V+E) log V) |
| 7 | Ubicaciones alcanzables | **BFS** | O(V + E) |
| 8 | Mostrar grafo | Recorrido de estructura | O(V + E) |
