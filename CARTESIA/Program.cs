using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

string rutaArchivo = "ubicaciones.txt";

const int MAX_UBICACIONES = 100;
Ubicacion[] ubicaciones = new Ubicacion[MAX_UBICACIONES];
int totalUbicaciones = 0;

cargarUbicaciones();
await administrarUbicaciones();

async Task administrarUbicaciones()
{
    int opcion;

    do
    {
        opcion = mostrarSubmenu();

        switch (opcion)
        {
            case 1: // Ver Ubicaciones
                mostrarArregloUbicaciones();
                break;

            case 2: // Agregar Ubicación
                await agregarUbicacionMenu();
                break;

            case 3: // Volver
                Console.WriteLine("Volviendo al menú anterior...");
                break;
        }

    } while (opcion != 3);
}

int mostrarSubmenu()
{
    int opcion;
    bool valido = false;

    do
    {
        Console.WriteLine("\n--- Administrar Ubicaciones ---");
        Console.WriteLine("1. Ver Ubicaciones");
        Console.WriteLine("2. Agregar Ubicación");
        Console.WriteLine("3. Volver");
        Console.Write("Seleccione una opción: ");

        if (int.TryParse(Console.ReadLine(), out opcion) && opcion >= 1 && opcion <= 3)
        {
            valido = true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Opción inválida. Intente nuevamente.");
            Console.ResetColor();
        }
    } while (!valido);

    return opcion;
}
void mostrarArregloUbicaciones()
{
    Console.WriteLine("\n--- Ubicaciones Registradas ---");

    if (totalUbicaciones == 0)
    {
        Console.WriteLine("No hay ubicaciones registradas.");
        return;
    }

    for (int i = 0; i < totalUbicaciones; i++)
    {
        Console.WriteLine($"{i + 1}. Nombre: {ubicaciones[i].nombre} | Latitud: {ubicaciones[i].latitud} | Longitud: {ubicaciones[i].longitud}");
    }
}

bool validarCoordenadas(double lat, double lon)
{
    const double minLat = 11.80;
    const double maxLat = 12.35;
    const double minLon = -86.50;
    const double maxLon = -85.90;

    return lat >= minLat && lat <= maxLat && lon >= minLon && lon <= maxLon;
}
void guardarUbicacionEnArchivo(string nombre, double lat, double lon)
{
    using StreamWriter archivo = new StreamWriter(rutaArchivo, append: true);
    archivo.WriteLine($"{nombre};{lat};{lon}");
}

void cargarUbicaciones()
{
    if (!File.Exists(rutaArchivo))
    {
        return;
    }

    StreamReader lector = new StreamReader(rutaArchivo);
    string? linea;

    while ((linea = lector.ReadLine()) != null && totalUbicaciones < MAX_UBICACIONES)
    {
        var partes = linea.Split(';');
        if (partes.Length >= 3 &&
            double.TryParse(partes[1], out double lat) &&
            double.TryParse(partes[2], out double lon))
        {
            ubicaciones[totalUbicaciones].nombre = partes[0];
            ubicaciones[totalUbicaciones].latitud = lat;
            ubicaciones[totalUbicaciones].longitud = lon;
            totalUbicaciones++;
        }
    }

    lector.Close();
}

async Task agregarUbicacionMenu()
{
    int opcion = mostrarSubmenuAgregar();

    switch (opcion)
    {
        case 1: // Buscar ubicación
            await buscarUbicacionPorApi();
            break;

        case 2: // Agregar manualmente
            agregarUbicacionManual();
            break;
    }
}

int mostrarSubmenuAgregar()
{
    int opcion;
    bool valido = false;

    do
    {
        Console.WriteLine("\n--- Agregar Ubicación ---");
        Console.WriteLine("1. Buscar ubicación");
        Console.WriteLine("2. Agregar manualmente");
        Console.Write("Seleccione una opción: ");

        if (int.TryParse(Console.ReadLine(), out opcion) && opcion >= 1 && opcion <= 2)
        {
            valido = true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Opción inválida. Intente nuevamente.");
            Console.ResetColor();
        }
    } while (!valido);

    return opcion;
}

void agregarUbicacionManual()
{
    if (totalUbicaciones >= MAX_UBICACIONES)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("No se pueden agregar más ubicaciones, el arreglo está lleno.");
        Console.ResetColor();
        return;
    }

    string nombre = ingresarNombreUbicacion();
    var (lat, lon) = ingresarLongitudLatitud();

    ubicaciones[totalUbicaciones].nombre = nombre;
    ubicaciones[totalUbicaciones].latitud = lat;
    ubicaciones[totalUbicaciones].longitud = lon;
    totalUbicaciones++;

    guardarUbicacionEnArchivo(nombre, lat, lon);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Ubicación agregada correctamente.");
    Console.ResetColor();
}

string ingresarNombreUbicacion()
{
    string nombre;
    bool valido = false;

    do
    {
        Console.Write("Ingrese el nombre de la ubicación: ");
        nombre = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            valido = true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("El nombre no puede estar vacío. Intente nuevamente.");
            Console.ResetColor();
        }
    } while (!valido);

    return nombre;
}

(double lat, double lon) ingresarLongitudLatitud()
{
    double lat, lon;
    bool valido = false;

    do
    {
        Console.Write("Ingrese la latitud: ");
        bool latValida = double.TryParse(Console.ReadLine(), out lat);

        Console.Write("Ingrese la longitud: ");
        bool lonValida = double.TryParse(Console.ReadLine(), out lon);

        if (!latValida || !lonValida)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Debe ingresar valores numéricos válidos.");
            Console.ResetColor();
            continue;
        }

        if (!validarCoordenadas(lat, lon))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Las coordenadas están fuera del área permitida (Managua y Masaya, Nicaragua). Intente nuevamente.");
            Console.ResetColor();
            continue;
        }

        valido = true;

    } while (!valido);

    return (lat, lon);
}

async Task buscarUbicacionPorApi()
{
    if (totalUbicaciones >= MAX_UBICACIONES)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("No se pueden agregar más ubicaciones, el arreglo está lleno.");
        Console.ResetColor();
        return;
    }

    bool valido = false;
    string lugar = "";
    do
    {
        Console.Write("Ingresa el nombre del lugar a buscar: ");
        lugar = Console.ReadLine();
        if (!string.IsNullOrEmpty(lugar))
        {
            valido = true;
        }
    } while (!valido);

    var parametros = new Dictionary<string, string>
    {
        ["type"] = "search",
        ["hl"] = "es",
        ["engine"] = "google_maps",
        ["api_key"] = "0f623482556b342af3e7117b162e8b72e6dbfbb1c2b50881a6e542bf3897f3d5",
        ["gl"] = "ni",
        ["q"] = lugar,
        ["ll"] = "@12.045,-86.19,11z"
    };

    using HttpClient client = new();

    string url = "https://serpapi.com/search.json?" +
                 string.Join("&", parametros.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));

    string respuesta;
    try
    {
        respuesta = await client.GetStringAsync(url);
    }
    catch (HttpRequestException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("No se pudo conectar con el servicio de búsqueda. Intente más tarde.");
        Console.ResetColor();
        return;
    }

    using JsonDocument doc = JsonDocument.Parse(respuesta);
    var raiz = doc.RootElement;

    if (!raiz.TryGetProperty("local_results", out var resultados))
    {
        Console.WriteLine("No se encontraron resultados para esta búsqueda en la zona.");
        return;
    }

    var lugaresEncontrados = new List<(string nombre, double lat, double lng)>();

    foreach (var negocio in resultados.EnumerateArray().Take(5))
    {
        string nombre = negocio.GetProperty("title").GetString();

        if (negocio.TryGetProperty("gps_coordinates", out var gps))
        {
            double lat = gps.GetProperty("latitude").GetDouble();
            double lng = gps.GetProperty("longitude").GetDouble();
            lugaresEncontrados.Add((nombre, lat, lng));
        }
    }

    lugaresEncontrados.RemoveAll(l => !validarCoordenadas(l.lat, l.lng));

    if (lugaresEncontrados.Count == 0)
    {
        Console.WriteLine("No se encontraron resultados dentro del área permitida (Managua y Masaya).");
        return;
    }

    Console.WriteLine("\n--- Resultados obtenidos ---");
    for (int i = 0; i < lugaresEncontrados.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {lugaresEncontrados[i].nombre}");
    }

    int seleccion;
    bool seleccionValida = false;
    do
    {
        Console.Write("Seleccione el lugar: ");
        if (int.TryParse(Console.ReadLine(), out seleccion) && seleccion >= 1 && seleccion <= lugaresEncontrados.Count)
        {
            seleccionValida = true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Opción inválida. Intente nuevamente.");
            Console.ResetColor();
        }
    } while (!seleccionValida);

    var elegido = lugaresEncontrados[seleccion - 1];

    ubicaciones[totalUbicaciones].nombre = elegido.nombre;
    ubicaciones[totalUbicaciones].latitud = elegido.lat;
    ubicaciones[totalUbicaciones].longitud = elegido.lng;
    totalUbicaciones++;

    guardarUbicacionEnArchivo(elegido.nombre, elegido.lat, elegido.lng);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Ubicación agregada correctamente desde la búsqueda.");
    Console.ResetColor();
}

// ===========================================================
// Módulo "Mostrar historial"
//
// Nota: esta función implementa ÚNICAMENTE la lógica del diagrama
// de flujo que es la parte que me toca hacer osea; (abrir archivo
// -> verificar si hay registros -> mostrar registros o mensaje -> fin).
// La interfaz/menú que invoque esta
// función la integra el compañero encargado de esa parte del
// proyecto; aquí solo se garantiza que el flujo funcione.
// al momento de escribir esto, no se ha
// definido el menú o interfaz que invoque esta función,
// por lo que no se incluye ningún código relacionado a eso,
// solo la función mostrarHistorial()
// y sus funciones auxiliares.
// ===========================================================

string rutaHistorial = "historial.txt";

// Inicio -> Abrir historial.txt -> Hay registros?
//   Si -> Mostrar registros -> FIN
//   No -> Mostrar mensaje -> FIN
void mostrarHistorial()
{
    // Abrir historial.txt
    if (!File.Exists(rutaHistorial))
    {
        // No hay registros (el archivo ni siquiera existe todavía)
        mostrarMensajeSinHistorial();
        return; // FIN
    }

    List<string> registros = leerRegistrosHistorial();

    // Hay registros?
    if (registros.Count > 0)
    {
        // Sí -> Mostrar registros
        mostrarRegistrosHistorial(registros);
    }
    else
    {
        // No -> Mostrar mensaje
        mostrarMensajeSinHistorial();
    }
    // FIN
}

// Lee historial.txt con StreamReader y devuelve cada línea no vacía
// como un registro.
List<string> leerRegistrosHistorial()
{
    var registros = new List<string>();

    StreamReader lector = new StreamReader(rutaHistorial);
    string? linea;

    while ((linea = lector.ReadLine()) != null)
    {
        if (!string.IsNullOrWhiteSpace(linea))
        {
            registros.Add(linea);
        }
    }

    lector.Close();

    return registros;
}

// Rama "Sí": Mostrar registros
void mostrarRegistrosHistorial(List<string> registros)
{
    Console.WriteLine("\n--- Historial ---");
    for (int i = 0; i < registros.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {registros[i]}");
    }
}

// Rama "No": Mostrar mensaje
void mostrarMensajeSinHistorial()
{
    Console.WriteLine("No hay registros en el historial.");
}

//feature/frank-estadisticas-viajes

void MostrarEstadisticas()
{
    if (!File.Exists(rutaHistorial) || File.ReadAllLines(rutaHistorial).Length == 0)
    {
        Console.WriteLine("\nNo hay registros en el historial.");
        return;
    }

    string[] registros = File.ReadAllLines(rutaHistorial);

    int totalViajes = registros.Length;
    double costoTotal = 0;
    double viajeMasCaro = double.MinValue;
    double viajeMasBarato = double.MaxValue;

    string destinoMasFrecuente = "";
    int mayorFrecuencia = 0;

    var destinos = new Dictionary<string, int>();

    foreach (string registro in registros)
    {
        string[] datos = registro.Split(';');

        if (datos.Length < 2)
        {
            continue;
        }

        string destino = datos[0];

        if (!double.TryParse(datos[1], out double costo))
        {
            continue;
        }

        costoTotal += costo;

        if (costo > viajeMasCaro)
            viajeMasCaro = costo;

        if (costo < viajeMasBarato)
            viajeMasBarato = costo;

        if (destinos.ContainsKey(destino))
            destinos[destino]++;
        else
            destinos[destino] = 1;
    }

    foreach (var destino in destinos)
    {
        if (destino.Value > mayorFrecuencia)
        {
            mayorFrecuencia = destino.Value;
            destinoMasFrecuente = destino.Key;
        }
    }

    double costoPromedio = costoTotal / totalViajes;

    Console.WriteLine("\n=== ESTADÍSTICAS ===");
    Console.WriteLine($"Total de viajes: {totalViajes}");
    Console.WriteLine($"Costo promedio: {costoPromedio:C}");
    Console.WriteLine($"Viaje más caro: {viajeMasCaro:C}");
    Console.WriteLine($"Viaje más barato: {viajeMasBarato:C}");
    Console.WriteLine($"Destino más frecuente: {destinoMasFrecuente}");
}


struct Ubicacion
{
    public string nombre;
    public double latitud;
    public double longitud;
}