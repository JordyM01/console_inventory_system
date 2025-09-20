using System.Text;

/// <summary>
/// Punto de entrada y orquestador principal de la aplicación.
/// Gestiona el bucle principal, el cambio de vistas y la detección de redimensionamiento de la ventana.
/// </summary>
public class Program
{
    private static int _lastWindowWidth = 0;
    private static int _lastWindowHeight = 0;

    public static void Main(string[] args)
    {
        // Configuración inicial de la consola
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.Title = "Sistema de Gestión de Inventario";
        Console.CursorVisible = false;

        // Se crea una única instancia del gestor de inventario que se compartirá entre todas las vistas
        var inventoryManager = new InventoryManager("inventory.dat");

        // La vista inicial es el menú principal
        IView currentView = new MainMenuView(inventoryManager);
        IView previousView = null; // Se usa para detectar cuándo se cambia de una vista a otra

        // Bucle principal de la aplicación: se ejecuta mientras haya una vista activa
        while (currentView != null)
        {
            // Si la instancia de la vista ha cambiado o la ventana fue redimensionada, se limpia la pantalla
            if (!ReferenceEquals(currentView, previousView) || WindowWasResized())
            {
                Console.Clear();
            }
            previousView = currentView; // Se actualiza la referencia a la vista anterior

            // 1. Dibuja la vista actual en la consola
            currentView.Draw();
            // 2. Espera a que el usuario presione una tecla
            ConsoleKeyInfo key = Console.ReadKey(true);
            // 3. Delega el manejo de la tecla a la vista actual, que devolverá la siguiente vista a mostrar
            currentView = currentView.HandleInput(key);
        }

        // Al salir del bucle, se limpia la consola y se muestra un mensaje de despedida
        Console.Clear();
        Console.CursorVisible = true;
        Console.WriteLine("¡Hasta luego!");
    }

    /// <summary>
    /// Comprueba si las dimensiones de la ventana han cambiado desde la última vez.
    /// </summary>
    /// <returns>True si la ventana fue redimensionada, de lo contrario False.</returns>
    private static bool WindowWasResized()
    {
        if (Console.WindowWidth != _lastWindowWidth || Console.WindowHeight != _lastWindowHeight)
        {
            _lastWindowWidth = Console.WindowWidth;
            _lastWindowHeight = Console.WindowHeight;
            return true;
        }
        return false;
    }
}

