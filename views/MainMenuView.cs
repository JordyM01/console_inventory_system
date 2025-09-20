using static UiComponents;

/// <summary>
/// Vista del menú principal. No usa el sistema de foco porque su único contenido es la navegación.
/// </summary>
public class MainMenuView : IView
{
    private readonly InventoryManager _inventoryManager;
    private int _navigationIndex = 1; // Inicia en "Agregar producto" (índice 1 de MenuItems)

    public MainMenuView(InventoryManager manager) => _inventoryManager = manager;

    public void Draw()
    {
        Console.CursorVisible = false;
        DrawBox(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);
        string title = "Sistema de gestion de inventario en consola";
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, 5);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(title);

        string[] menuOptions = { "Agregar producto", "Mostrar productos", "Actualizar producto", "Eliminar producto", "Acerca del software", "Salir" };
        for (int i = 0; i < menuOptions.Length; i++)
        {
            int col = i < 3 ? 0 : 1;
            int row = i % 3;
            int x = Console.WindowWidth / 4 * (col + 1) - 15;
            int y = 10 + row * 2;
            Console.SetCursorPosition(x, y);

            if (i == _navigationIndex - 1) // Ajusta el índice para el array 0-based
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($" >> {menuOptions[i]} ");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"    {menuOptions[i]} ");
            }
        }
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
        int numItems = NavigationHelper.MenuItems.Length - 1;
        switch (key.Key)
        {
            case ConsoleKey.UpArrow: _navigationIndex = (_navigationIndex > 1) ? _navigationIndex - 1 : numItems; break;
            case ConsoleKey.DownArrow: _navigationIndex = (_navigationIndex < numItems) ? _navigationIndex + 1 : 1; break;
            case ConsoleKey.RightArrow: if (_navigationIndex <= 3) _navigationIndex += 3; break;
            case ConsoleKey.LeftArrow: if (_navigationIndex > 3) _navigationIndex -= 3; break;

            case ConsoleKey.Enter:
                return NavigationHelper.GetViewByIndex(_navigationIndex, _inventoryManager);

            case ConsoleKey.Escape:
                return NavigationHelper.GetViewByIndex(6, _inventoryManager); // Salir
        }
        return this;
    }
}


