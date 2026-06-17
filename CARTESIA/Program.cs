using System;
using System.IO;

string rutaArchivo = "ubicaciones.txt";

const int MAX_UBICACIONES = 100;
Ubicacion[] ubicaciones = new Ubicacion[MAX_UBICACIONES];
int totalUbicaciones = 0;

cargarUbicaciones();
administrarUbicaciones();

void administrarUbicaciones()
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
                agregarUbicacionMenu();
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

void agregarUbicacionMenu()
{
    int opcion = mostrarSubmenuAgregar();

    switch (opcion)
    {
        case 1: // Buscar ubicación
            Console.WriteLine("Pendiente: búsqueda por API.");
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
struct Ubicacion
{
    public string nombre;
    public double latitud;
    public double longitud;
}