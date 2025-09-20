using System.Text;

// --- Punto de entrada y Bucle Principal de la Aplicación ---
public class Program
{
    private static int _lastWindowWidth = 0;
    private static int _lastWindowHeight = 0;

    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.Title = "Sistema de Gestión de Inventario";
        Console.CursorVisible = false;

        var inventoryManager = new InventoryManager("inventory.dat");
        IView currentView = new MainMenuView(inventoryManager);
        IView previousView = null; // Variable para rastrear la vista anterior

        while (currentView != null)
        {
            // Si la instancia de la vista ha cambiado o la ventana fue redimensionada, limpia la pantalla.
            // Usamos ReferenceEquals para comparar las instancias de los objetos.
            if (!ReferenceEquals(currentView, previousView) || WindowWasResized())
            {
                Console.Clear();
            }
            previousView = currentView; // Actualiza la vista anterior

            currentView.Draw();
            ConsoleKeyInfo key = Console.ReadKey(true);
            currentView = currentView.HandleInput(key);
        }

        Console.Clear();
        Console.CursorVisible = true;
        Console.WriteLine("¡Hasta luego!");
    }

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

