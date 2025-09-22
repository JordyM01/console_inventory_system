using System.Text;

/// <summary>
/// Punto de entrada y orquestador principal de la aplicación.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Sistema de Gestión de Inventario";
        Console.CursorVisible = false;

        var inventoryManager = new InventoryManager("inventory.json");
        var renderer = new TuiRenderer();

        IView? currentView = new MainMenuView(inventoryManager);

        while (currentView != null)
        {
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



