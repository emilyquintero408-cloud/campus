using System;
using System.Collections.Generic;
using System.Linq;

namespace CampusRutas
{
    /// <summary>
    /// Grafo no dirigido con pesos que modela el campus universitario.
    /// Representación interna: lista de adyacencia con diccionarios.
    /// </summary>
    public class Grafo
    {
        // Lista de adyacencia: cada nodo mapea a sus vecinos con sus pesos
        private Dictionary<string, Dictionary<string, double>> _adyacencia;

        public Grafo()
        {
            _adyacencia = new Dictionary<string, Dictionary<string, double>>(StringComparer.OrdinalIgnoreCase);
        }

        // ─────────────────────────────────────────────────────────────
        // OPERACIONES SOBRE UBICACIONES (NODOS)
        // ─────────────────────────────────────────────────────────────

        /// <summary>Agrega una ubicación al grafo. O(1)</summary>
        public bool AgregarUbicacion(string nombre)
        {
            nombre = nombre.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                Console.WriteLine("[Error] El nombre no puede estar vacío.");
                return false;
            }
            if (_adyacencia.ContainsKey(nombre))
            {
                Console.WriteLine($"[Aviso] La ubicación '{nombre}' ya existe.");
                return false;
            }
            _adyacencia[nombre] = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            Console.WriteLine($"[OK] Ubicación '{nombre}' agregada.");
            return true;
        }

        /// <summary>Elimina una ubicación y todas sus rutas. O(V + E)</summary>
        public bool EliminarUbicacion(string nombre)
        {
            if (!_adyacencia.ContainsKey(nombre))
            {
                Console.WriteLine($"[Error] La ubicación '{nombre}' no existe.");
                return false;
            }
            // Remover referencias en otros nodos
            foreach (var vecinos in _adyacencia.Values)
                vecinos.Remove(nombre);

            _adyacencia.Remove(nombre);
            Console.WriteLine($"[OK] Ubicación '{nombre}' eliminada junto con sus rutas.");
            return true;
        }

        /// <summary>Verifica si una ubicación existe. O(1)</summary>
        public bool ExisteUbicacion(string nombre) => _adyacencia.ContainsKey(nombre);

        /// <summary>Retorna la lista de todas las ubicaciones. O(V)</summary>
        public List<string> ObtenerUbicaciones() => _adyacencia.Keys.ToList();

        // ─────────────────────────────────────────────────────────────
        // OPERACIONES SOBRE RUTAS (ARISTAS)
        // ─────────────────────────────────────────────────────────────

        /// <summary>Agrega una ruta bidireccional con peso en metros. O(1)</summary>
        public bool AgregarRuta(string origen, string destino, double metros)
        {
            if (!ValidarUbicaciones(origen, destino)) return false;
            if (origen.Equals(destino, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("[Error] El origen y destino no pueden ser iguales.");
                return false;
            }
            if (metros <= 0)
            {
                Console.WriteLine("[Error] El peso debe ser mayor a 0.");
                return false;
            }
            _adyacencia[origen][destino] = metros;
            _adyacencia[destino][origen] = metros;
            Console.WriteLine($"[OK] Ruta '{origen}' ↔ '{destino}' ({metros} m) agregada.");
            return true;
        }

        /// <summary>Elimina una ruta bidireccional. O(1)</summary>
        public bool EliminarRuta(string origen, string destino)
        {
            if (!ValidarUbicaciones(origen, destino)) return false;
            if (!_adyacencia[origen].ContainsKey(destino))
            {
                Console.WriteLine($"[Error] No existe ruta entre '{origen}' y '{destino}'.");
                return false;
            }
            _adyacencia[origen].Remove(destino);
            _adyacencia[destino].Remove(origen);
            Console.WriteLine($"[OK] Ruta '{origen}' ↔ '{destino}' eliminada.");
            return true;
        }

        // ─────────────────────────────────────────────────────────────
        // ALGORITMO DE DIJKSTRA – Ruta más corta
        // Complejidad: O((V + E) log V) con cola de prioridad
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Calcula la ruta más corta entre dos ubicaciones usando Dijkstra.
        /// Retorna (distancia, camino) o (-1, null) si no hay camino.
        /// </summary>
        public (double distancia, List<string> camino) RutaMasCorta(string origen, string destino)
        {
            if (!ValidarUbicaciones(origen, destino)) return (-1, null);

            var distancias = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            var anteriores = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var visitados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Inicializar todas las distancias como infinito
            foreach (var nodo in _adyacencia.Keys)
                distancias[nodo] = double.MaxValue;
            distancias[origen] = 0;

            // Cola de prioridad: (distancia, nodo)
            var cola = new SortedSet<(double dist, string nodo)>(
                Comparer<(double, string)>.Create((a, b) =>
                    a.Item1 != b.Item1 ? a.Item1.CompareTo(b.Item1) : string.Compare(a.Item2, b.Item2, StringComparison.OrdinalIgnoreCase)));

            cola.Add((0, origen));

            while (cola.Count > 0)
            {
                var (distActual, nodoActual) = cola.Min;
                cola.Remove(cola.Min);

                if (visitados.Contains(nodoActual)) continue;
                visitados.Add(nodoActual);

                if (nodoActual.Equals(destino, StringComparison.OrdinalIgnoreCase)) break;

                foreach (var (vecino, peso) in _adyacencia[nodoActual])
                {
                    if (visitados.Contains(vecino)) continue;
                    double nuevaDist = distActual + peso;
                    if (nuevaDist < distancias[vecino])
                    {
                        distancias[vecino] = nuevaDist;
                        anteriores[vecino] = nodoActual;
                        cola.Add((nuevaDist, vecino));
                    }
                }
            }

            if (distancias[destino] == double.MaxValue)
                return (-1, null); // No hay camino

            // Reconstruir el camino
            var camino = new List<string>();
            string actual = destino;
            while (actual != null)
            {
                camino.Insert(0, actual);
                anteriores.TryGetValue(actual, out actual);
            }

            return (distancias[destino], camino);
        }

        // ─────────────────────────────────────────────────────────────
        // BFS – Ubicaciones alcanzables desde un origen
        // Complejidad: O(V + E)
        // ─────────────────────────────────────────────────────────────

        /// <summary>Lista todas las ubicaciones alcanzables desde un origen usando BFS.</summary>
        public List<string> UbicacionesAlcanzables(string origen)
        {
            if (!ExisteUbicacion(origen))
            {
                Console.WriteLine($"[Error] La ubicación '{origen}' no existe.");
                return new List<string>();
            }

            var visitados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var cola = new Queue<string>();
            var resultado = new List<string>();

            cola.Enqueue(origen);
            visitados.Add(origen);

            while (cola.Count > 0)
            {
                string actual = cola.Dequeue();
                if (!actual.Equals(origen, StringComparison.OrdinalIgnoreCase))
                    resultado.Add(actual);

                foreach (var vecino in _adyacencia[actual].Keys)
                {
                    if (!visitados.Contains(vecino))
                    {
                        visitados.Add(vecino);
                        cola.Enqueue(vecino);
                    }
                }
            }

            return resultado;
        }

        // ─────────────────────────────────────────────────────────────
        // VISUALIZACIÓN DEL GRAFO
        // ─────────────────────────────────────────────────────────────

        /// <summary>Muestra la lista de adyacencia completa del grafo.</summary>
        public void MostrarListaAdyacencia()
        {
            if (_adyacencia.Count == 0)
            {
                Console.WriteLine("[Aviso] El grafo está vacío.");
                return;
            }

            Console.WriteLine("\n╔══════════════════════════════════════════╗");
            Console.WriteLine("║         LISTA DE ADYACENCIA              ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");

            foreach (var nodo in _adyacencia.Keys.OrderBy(k => k))
            {
                Console.Write($"  {nodo,-25} → ");
                if (_adyacencia[nodo].Count == 0)
                    Console.WriteLine("(sin conexiones)");
                else
                    Console.WriteLine(string.Join(", ", _adyacencia[nodo]
                        .OrderBy(v => v.Key)
                        .Select(v => $"{v.Key} ({v.Value}m)")));
            }
            Console.WriteLine();
        }

        /// <summary>Retorna estadísticas básicas del grafo.</summary>
        public (int vertices, int aristas) ObtenerEstadisticas()
        {
            int aristas = _adyacencia.Values.Sum(v => v.Count) / 2;
            return (_adyacencia.Count, aristas);
        }

        // ─────────────────────────────────────────────────────────────
        // AUXILIARES
        // ─────────────────────────────────────────────────────────────

        private bool ValidarUbicaciones(string a, string b)
        {
            if (!_adyacencia.ContainsKey(a))
            {
                Console.WriteLine($"[Error] La ubicación '{a}' no existe.");
                return false;
            }
            if (!_adyacencia.ContainsKey(b))
            {
                Console.WriteLine($"[Error] La ubicación '{b}' no existe.");
                return false;
            }
            return true;
        }
    }
}
