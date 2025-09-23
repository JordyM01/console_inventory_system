using System.Text;

/// <summary>
/// Punto de entrada y orquestador principal de la aplicación.
/// </summary>
public class Program
{
    private static int _lastWindowWidth = 0;
    private static int _lastWindowHeight = 0;

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Sistema de Gestión de Inventario";
        Console.CursorVisible = false;

        // Se establece un color de fondo consistente para toda la aplicación
        Console.BackgroundColor = ConsoleColor.Black;
        // Se limpia la consola para asegurar que la aplicación inicie en una pantalla limpia.
        Console.Clear();

        var inventoryManager = new InventoryManager("inventory.json");
        var renderer = new TuiRenderer();

        IView? currentView = new MainMenuView(inventoryManager);

        while (currentView != null)
        {
            // Detecta si la ventana ha sido redimensionada por el usuario.
            if (Console.WindowWidth != _lastWindowWidth || Console.WindowHeight != _lastWindowHeight)
            {
                _lastWindowWidth = Console.WindowWidth;
                _lastWindowHeight = Console.WindowHeight;

                // Si cambió el tamaño, se fuerza una limpieza y se reconstruye la vista actual
                // para que todos sus componentes recalculen sus posiciones.
                Console.Clear();
                currentView = new MainMenuView(inventoryManager);
            }

            renderer.ClearBuffer();
            currentView.Draw(renderer);
            renderer.Render();

            ConsoleKeyInfo key = Console.ReadKey(true);
            currentView = currentView.HandleInput(key);
        }

        Console.Clear();
        Console.CursorVisible = true;
        Console.WriteLine("¡Hasta luego!");
    }
}


