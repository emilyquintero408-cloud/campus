using System;
using System.Collections.Generic;
using System.IO;

namespace CampusRutas
{
    class Program
    {
        static Grafo grafo = new Grafo();
        static string archivoDatos = Path.Combine("..", "data", "campus.txt");

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            MostrarBienvenida();

            // Carga automática si el archivo existe
            if (File.Exists(archivoDatos))
            {
                Console.WriteLine($"Cargando datos desde '{archivoDatos}'...\n");
                CargadorArchivo.CargarDesdeArchivo(grafo, archivoDatos);
            }
            else
            {
                Console.WriteLine("[Aviso] No se encontró archivo de datos. Inicia con un grafo vacío.\n");
            }

            bool salir = false;
            while (!salir)
            {
                MostrarMenu();
                string opcion = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                switch (opcion)
                {
                    case "1": CargarArchivo(); break;
                    case "2": AgregarUbicacion(); break;
                    case "3": EliminarUbicacion(); break;
                    case "4": AgregarRuta(); break;
                    case "5": EliminarRuta(); break;
                    case "6": RutaMasCorta(); break;
                    case "7": UbicacionesAlcanzables(); break;
                    case "8": MostrarGrafo(); break;
                    case "9": ListarUbicaciones(); break;
                    case "0":
                        salir = true;
                        Console.WriteLine("¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("[Error] Opción inválida. Intenta de nuevo.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresiona Enter para continuar...");
                    Console.ReadLine();
                }
            }
        }

        // ─── MENÚ ───────────────────────────────────────────────────

        static void MostrarBienvenida()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║      BUSCADOR DE RUTAS DEL CAMPUS            ║");
            Console.WriteLine("║   Estructuras de Datos – Grafos con Dijkstra ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        static void MostrarMenu()
        {
            Console.Clear();
            var (v, e) = grafo.ObtenerEstadisticas();
            Console.WriteLine($"╔══════════════════════════════════════╗");
            Console.WriteLine($"║  MENÚ PRINCIPAL  [{v} nodos | {e} rutas]");
            Console.WriteLine($"╠══════════════════════════════════════╣");
            Console.WriteLine($"║  1. Cargar campus desde archivo      ║");
            Console.WriteLine($"║  2. Agregar ubicación                ║");
            Console.WriteLine($"║  3. Eliminar ubicación               ║");
            Console.WriteLine($"║  4. Agregar ruta entre ubicaciones   ║");
            Console.WriteLine($"║  5. Eliminar ruta entre ubicaciones  ║");
            Console.WriteLine($"║  6. Ruta más corta (Dijkstra)        ║");
            Console.WriteLine($"║  7. Ubicaciones alcanzables (BFS)    ║");
            Console.WriteLine($"║  8. Mostrar lista de adyacencia      ║");
            Console.WriteLine($"║  9. Listar todas las ubicaciones     ║");
            Console.WriteLine($"║  0. Salir                            ║");
            Console.WriteLine($"╚══════════════════════════════════════╝");
            Console.Write("Opción: ");
        }

        // ─── HANDLERS ───────────────────────────────────────────────

        static void CargarArchivo()
        {
            Console.Write("Ruta del archivo (Enter para usar default): ");
            string ruta = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(ruta)) ruta = archivoDatos;
            CargadorArchivo.CargarDesdeArchivo(grafo, ruta);
        }

        static void AgregarUbicacion()
        {
            Console.Write("Nombre de la ubicación: ");
            string nombre = Console.ReadLine() ?? "";
            grafo.AgregarUbicacion(nombre);
        }

        static void EliminarUbicacion()
        {
            Console.Write("Nombre de la ubicación a eliminar: ");
            string nombre = Console.ReadLine() ?? "";
            grafo.EliminarUbicacion(nombre);
        }

        static void AgregarRuta()
        {
            Console.Write("Ubicación origen: ");
            string origen = Console.ReadLine() ?? "";
            Console.Write("Ubicación destino: ");
            string destino = Console.ReadLine() ?? "";
            Console.Write("Distancia en metros: ");
            if (double.TryParse(Console.ReadLine(), out double metros))
                grafo.AgregarRuta(origen, destino, metros);
            else
                Console.WriteLine("[Error] Distancia inválida.");
        }

        static void EliminarRuta()
        {
            Console.Write("Ubicación origen: ");
            string origen = Console.ReadLine() ?? "";
            Console.Write("Ubicación destino: ");
            string destino = Console.ReadLine() ?? "";
            grafo.EliminarRuta(origen, destino);
        }

        static void RutaMasCorta()
        {
            Console.Write("Origen: ");
            string origen = Console.ReadLine() ?? "";
            Console.Write("Destino: ");
            string destino = Console.ReadLine() ?? "";

            var (distancia, camino) = grafo.RutaMasCorta(origen, destino);

            if (distancia < 0)
            {
                Console.WriteLine($"\n[Resultado] No existe camino entre '{origen}' y '{destino}'.");
                return;
            }

            Console.WriteLine($"\n╔══════════════════════════════════════╗");
            Console.WriteLine($"║  RUTA MÁS CORTA (Dijkstra)           ║");
            Console.WriteLine($"╚══════════════════════════════════════╝");
            Console.WriteLine($"  Distancia total: {distancia} metros");
            Console.WriteLine($"  Pasos: {camino.Count - 1}");
            Console.WriteLine($"  Camino: {string.Join(" → ", camino)}");
        }

        static void UbicacionesAlcanzables()
        {
            Console.Write("Ubicación de origen: ");
            string origen = Console.ReadLine() ?? "";

            List<string> alcanzables = grafo.UbicacionesAlcanzables(origen);

            if (alcanzables.Count == 0)
            {
                Console.WriteLine($"\n[Resultado] '{origen}' no tiene ubicaciones alcanzables.");
                return;
            }

            Console.WriteLine($"\n╔══════════════════════════════════════╗");
            Console.WriteLine($"║  UBICACIONES ALCANZABLES DESDE '{origen}'");
            Console.WriteLine($"╚══════════════════════════════════════╝");
            for (int i = 0; i < alcanzables.Count; i++)
                Console.WriteLine($"  {i + 1,2}. {alcanzables[i]}");
            Console.WriteLine($"\n  Total: {alcanzables.Count} ubicaciones");
        }

        static void MostrarGrafo()
        {
            grafo.MostrarListaAdyacencia();
        }

        static void ListarUbicaciones()
        {
            var ubicaciones = grafo.ObtenerUbicaciones();
            if (ubicaciones.Count == 0)
            {
                Console.WriteLine("[Aviso] El grafo no tiene ubicaciones.");
                return;
            }
            Console.WriteLine("\n╔══════════════════════════════════════╗");
            Console.WriteLine("║       UBICACIONES DEL CAMPUS         ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            ubicaciones.Sort();
            for (int i = 0; i < ubicaciones.Count; i++)
                Console.WriteLine($"  {i + 1,2}. {ubicaciones[i]}");
        }
    }
}
