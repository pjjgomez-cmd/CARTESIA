using System;

const int MAX_UBICACIONES = 100;
Ubicacion[] ubicaciones = new Ubicacion[MAX_UBICACIONES];
int totalUbicaciones = 0;

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
                Console.WriteLine("Pendiente: agregar ubicación.");
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
struct Ubicacion
{
    public string nombre;
    public double latitud;
    public double longitud;
}