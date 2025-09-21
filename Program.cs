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
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.Title = "Sistema de Gestión de Inventario";
        Console.CursorVisible = false;

        var inventoryManager = new InventoryManager("inventory.json");

        IView? currentView = new MainMenuView(inventoryManager);
        IView? previousView = null;

        while (currentView != null)
        {
            if (!ReferenceEquals(currentView, previousView) || WindowWasResized())
            {
                Console.Clear();
            }
            previousView = currentView;

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


