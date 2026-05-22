using System;
using System.IO;

namespace CampusRutas
{
    /// <summary>
    /// Carga ubicaciones y rutas desde archivo de texto.
    /// 
    /// Formato del archivo:
    ///   UBICACION NombreLugar
    ///   RUTA Origen;Destino;Metros
    /// </summary>
    public static class CargadorArchivo
    {
        public static bool CargarDesdeArchivo(Grafo grafo, string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                Console.WriteLine($"[Error] Archivo no encontrado: {rutaArchivo}");
                return false;
            }

            int linea = 0;
            int errores = 0;

            try
            {
                foreach (string lineasRaw in File.ReadAllLines(rutaArchivo))
                {
                    linea++;
                    string lineaTexto = lineasRaw.Trim();

                    // Ignorar líneas vacías y comentarios
                    if (string.IsNullOrEmpty(lineaTexto) || lineaTexto.StartsWith("#"))
                        continue;

                    string[] partes = lineaTexto.Split(' ', 2);
                    if (partes.Length < 2)
                    {
                        Console.WriteLine($"[Línea {linea}] Formato inválido: '{lineaTexto}'");
                        errores++;
                        continue;
                    }

                    string tipo = partes[0].ToUpper();
                    string datos = partes[1].Trim();

                    if (tipo == "UBICACION")
                    {
                        grafo.AgregarUbicacion(datos);
                    }
                    else if (tipo == "RUTA")
                    {
                        string[] campos = datos.Split(';');
                        if (campos.Length != 3)
                        {
                            Console.WriteLine($"[Línea {linea}] RUTA requiere: Origen;Destino;Metros");
                            errores++;
                            continue;
                        }
                        if (!double.TryParse(campos[2].Trim(), out double metros))
                        {
                            Console.WriteLine($"[Línea {linea}] Peso inválido: '{campos[2]}'");
                            errores++;
                            continue;
                        }
                        grafo.AgregarRuta(campos[0].Trim(), campos[1].Trim(), metros);
                    }
                    else
                    {
                        Console.WriteLine($"[Línea {linea}] Tipo desconocido: '{tipo}'");
                        errores++;
                    }
                }

                var (v, e) = grafo.ObtenerEstadisticas();
                Console.WriteLine($"\n[Carga completada] {v} ubicaciones, {e} rutas. Errores: {errores}");
                return errores == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error al leer archivo] {ex.Message}");
                return false;
            }
        }
    }
}
