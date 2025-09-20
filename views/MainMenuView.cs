using static UiComponents;

// --- Vista del Menú Principal ---
public class MainMenuView : IView
{
    private readonly InventoryManager _inventoryManager;
    private int _selectedIndex = 0;
    private readonly string[] _menuOptions = {
        "Agregar producto", "Mostrar productos", "Actualizar producto",
        "Eliminar producto", "Acerca del software", "Salir"
    };

    public MainMenuView(InventoryManager manager)
    {
        _inventoryManager = manager;
    }

    public void Draw()
    {
        DrawBox(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);
        string title = "Sistema de gestion de inventario en consola";
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, 5);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(title);

        for (int i = 0; i < _menuOptions.Length; i++)
        {
            int col = i < 3 ? 0 : 1;
            int row = i % 3;
            int x = Console.WindowWidth / 4 * (col + 1) - 15;
            int y = 10 + row * 2;
            Console.SetCursorPosition(x, y);

            if (i == _selectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($" >> {_menuOptions[i]} ");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"    {_menuOptions[i]} ");
            }
        }
        Console.ResetColor();
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow: _selectedIndex = (_selectedIndex > 0) ? _selectedIndex - 1 : _menuOptions.Length - 1; break;
            case ConsoleKey.DownArrow: _selectedIndex = (_selectedIndex < _menuOptions.Length - 1) ? _selectedIndex + 1 : 0; break;
            case ConsoleKey.RightArrow: if (_selectedIndex < 3) _selectedIndex += 3; break;
            case ConsoleKey.LeftArrow: if (_selectedIndex >= 3) _selectedIndex -= 3; break;
            case ConsoleKey.Enter:
                return ExecuteSelectedAction();
        }
        return this;
    }

    private IView ExecuteSelectedAction()
    {
        switch (_selectedIndex)
        {
            case 0: return new AddProductView(_inventoryManager);
            case 1: return new ShowProductsView(_inventoryManager);
            case 2: return new UpdateProductView(_inventoryManager);
            case 3: return new DeleteProductView(_inventoryManager);
            case 4: return new AboutView(_inventoryManager); // <-- Añadido
            case 5: return null; // Salir
            default: return this;
        }
    }
}


