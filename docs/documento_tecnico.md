# Documento Técnico – Buscador de Rutas del Campus

**Asignatura:** Estructuras de Datos  
**Problema:** 1 – Buscador de rutas del campus  
**Integrantes:** [Nombre 1] · [Nombre 2]  
**Lenguaje:** C# (.NET 8)

---

## 1. Descripción del problema y motivación

En un campus universitario, los estudiantes y visitantes necesitan encontrar caminos eficientes entre distintos puntos de interés: bloques de aulas, laboratorios, cafeterías, parqueaderos, etc. Navegar un campus desconocido sin orientación puede resultar confuso y costoso en tiempo.

Este proyecto propone un **sistema de navegación interna del campus** que permite:

- Modelar digitalmente la topología del campus como un grafo.
- Calcular la ruta más corta entre dos puntos en metros.
- Identificar qué ubicaciones son alcanzables desde cualquier punto.
- Mantener el mapa actualizado mediante operaciones de inserción y eliminación.

La motivación principal es aplicar los conceptos de **grafos con peso** y el algoritmo de **Dijkstra** en un problema real y tangible para cualquier estudiante universitario.

---

## 2. Modelado del problema con el grafo

### Tipo de grafo

| Propiedad | Valor | Justificación |
|-----------|-------|---------------|
| Dirigido / No dirigido | **No dirigido** | Si se puede ir de A a B, también se puede ir de B a A |
| Con / Sin peso | **Con peso** | Cada ruta tiene una distancia en metros |
| Con / Sin ciclos | **Con ciclos** | El campus tiene múltiples caminos entre zonas |

### Elementos del grafo

**Nodos (vértices):** Ubicaciones del campus

```
Entrada Principal
Biblioteca
Bloque A - Aulas
Bloque B - Laboratorios
Bloque C - Administrativo
Cafetería Central
Cancha Deportiva
Parqueadero
Auditorio
Bienestar Universitario
Sala de Sistemas
```

**Aristas:** Rutas peatonales directas entre dos ubicaciones, etiquetadas con metros.

### Diagrama del grafo (representación textual)

```
Parqueadero ──(80)── Entrada Principal ──(120)── Bloque C - Administrativo
     |                      |                              |
   (220)                  (200)                          (95)
     |                      |                              |
Cancha Deportiva        Biblioteca ──────(90)───── Bloque A - Aulas
     |                  /       \                  /    |     \
   (160)             (110)      (90)           (60)   (150)   (70)
     |               /              \          /       |        \
Cafetería Central ──/       Bloque B - Lab. ──┘   Cafetería   Sala Sistemas
     |  \                       |    \                             |
   (100) (180)                (130)  (50)                       (50)
     |     \                    |      \                          |
Bienestar   Bloque C          Auditorio  Sala de Sistemas ←──────┘
     |
   (140)
     |
  Auditorio
```

### Arreglos auxiliares

| Arreglo / Estructura | Tipo | Uso |
|----------------------|------|-----|
| `distancias` | `Dictionary<string, double>` | Distancias mínimas en Dijkstra |
| `anteriores` | `Dictionary<string, string>` | Reconstrucción del camino en Dijkstra |
| `visitados` | `HashSet<string>` | Control de nodos procesados en BFS/Dijkstra |
| `cola` (BFS) | `Queue<string>` | Cola FIFO para el recorrido BFS |
| `cola` (Dijkstra) | `SortedSet<(double, string)>` | Cola de prioridad ordenada por distancia |

---

## 3. Estructuras de datos utilizadas

### Representación del grafo: Lista de adyacencia

```csharp
Dictionary<string, Dictionary<string, double>> _adyacencia;
// nodo → { vecino → peso }
```

**Justificación:** Se eligió lista de adyacencia sobre matriz de adyacencia porque:

- El campus tiene pocos caminos directos entre la mayoría de pares de nodos (grafo **disperso**).
- La lista de adyacencia ocupa O(V + E) en memoria vs. O(V²) de la matriz.
- El acceso a los vecinos de un nodo es directo y eficiente con `Dictionary`.
- Permite nombres de ubicaciones como strings sin necesidad de índices numéricos.

### ¿Por qué `Dictionary` y no arreglo/lista?

- Los nombres de los nodos son strings arbitrarios, no índices numéricos.
- `Dictionary` garantiza O(1) para búsqueda, inserción y eliminación.
- Facilita la carga dinámica desde archivo sin conocer el tamaño del grafo de antemano.

---

## 4. Operaciones implementadas

### 4.1 Cargar desde archivo
- **Descripción:** Lee un archivo `.txt` línea por línea e interpreta comandos `UBICACION` y `RUTA`.
- **Complejidad:** O(V + E) donde V = ubicaciones y E = rutas en el archivo.

### 4.2 Agregar ubicación
- **Descripción:** Inserta un nuevo nodo al diccionario de adyacencia.
- **Validación:** Nombre no vacío, no duplicado.
- **Complejidad:** O(1) amortizado.

### 4.3 Eliminar ubicación
- **Descripción:** Elimina el nodo y recorre todos los demás nodos para borrar referencias a él.
- **Complejidad:** O(V + E) en el peor caso.

### 4.4 Agregar ruta
- **Descripción:** Inserta la arista en ambas direcciones (grafo no dirigido).
- **Validación:** Ambos nodos deben existir, peso > 0, no ser el mismo nodo.
- **Complejidad:** O(1).

### 4.5 Eliminar ruta
- **Descripción:** Elimina la arista en ambas direcciones.
- **Complejidad:** O(1).

### 4.6 Ruta más corta – Dijkstra
- **Descripción:** Calcula el camino de menor distancia total entre dos ubicaciones.
- **Implementación:** Cola de prioridad (`SortedSet`) ordenada por distancia acumulada.
- **Reconstrucción:** Diccionario `anteriores` que rastrea el predecesor de cada nodo.
- **Complejidad:** O((V + E) log V).

### 4.7 Ubicaciones alcanzables – BFS
- **Descripción:** Desde un nodo origen, recorre el grafo en anchura y retorna todos los nodos visitados.
- **Complejidad:** O(V + E).

### 4.8 Mostrar lista de adyacencia
- **Descripción:** Imprime en consola la representación interna completa.
- **Complejidad:** O(V + E).

---

## 5. Casos de prueba

### Caso 1 – Carga del campus
**Entrada:** archivo `data/campus.txt`  
**Salida esperada:**
```
[OK] Ubicación 'Entrada Principal' agregada.
...
[Carga completada] 11 ubicaciones, 16 rutas. Errores: 0
```

### Caso 2 – Ruta más corta: Parqueadero → Sala de Sistemas
**Entrada:** origen = Parqueadero, destino = Sala de Sistemas  
**Salida esperada:**
```
Distancia total: 310 metros
Camino: Parqueadero → Entrada Principal → Biblioteca → Bloque A - Aulas → Sala de Sistemas
```

### Caso 3 – Ubicaciones alcanzables desde Auditorio
**Entrada:** origen = Auditorio  
**Salida esperada:** 10 ubicaciones (todas, ya que el grafo es conexo)

### Caso 4 – Nodo aislado
**Entrada:** Agregar "Jardín Botánico" sin rutas, luego buscar alcanzables  
**Salida esperada:**
```
'Jardín Botánico' no tiene ubicaciones alcanzables.
```

### Caso 5 – Eliminar ubicación con rutas
**Entrada:** Eliminar "Cafetería Central"  
**Salida esperada:** Nodo eliminado y sus 5 rutas removidas automáticamente del grafo.

### Caso 6 – Validaciones de error
**Entrada:** AgregarRuta("Biblioteca", "Inexistente", 100)  
**Salida esperada:**
```
[Error] La ubicación 'Inexistente' no existe.
```

---

## 6. Instrucciones de ejecución

```bash
# Requisito: .NET 8 SDK instalado
cd src
dotnet run
```

El menú interactivo ofrece 9 opciones numeradas. El campus de ejemplo se carga automáticamente desde `data/campus.txt`.

Para usar un campus personalizado, crea un archivo `.txt` con el formato:
```
UBICACION NombreLugar
RUTA Lugar1;Lugar2;Metros
```

---

## 7. Limitaciones y posibles mejoras

### Limitaciones actuales

- La interfaz es solo de consola (sin visualización gráfica del mapa).
- No persiste cambios en tiempo de ejecución al archivo original.
- Asume que todos los nombres de ubicaciones son únicos (sin ID numérico).
- No implementa coordenadas geográficas reales.

### Posibles mejoras

- Agregar coordenadas X/Y a cada nodo para visualizar el grafo en pantalla.
- Implementar el algoritmo A* usando coordenadas como heurística para mayor eficiencia.
- Exportar el estado actual del grafo a un nuevo archivo `.txt`.
- Interfaz gráfica con mapa visual interactivo.
- Soporte para rutas con tráfico (peso variable según hora del día).
- Detectar y reportar si el grafo quedó desconectado tras una eliminación.
