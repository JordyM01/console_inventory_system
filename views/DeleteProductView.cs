using static UiComponents;

// --- Vista para Eliminar Productos ---
public class DeleteProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private int _selectedIndex = 0;

    public DeleteProductView(InventoryManager manager)
    {
        _inventoryManager = manager;
    }

    public void Draw()
    {
        DrawLayout("Eliminar producto");

        int contentX = 27;
        int contentY = 3;

        Console.SetCursorPosition(contentX, contentY);
        Console.Write("/ Eliminar producto");

        int tableY = contentY + 5;
        DrawBox(contentX, tableY, Console.WindowWidth - contentX - 2, Console.WindowHeight - tableY - 2);

        Console.SetCursorPosition(contentX + 2, tableY + 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Producto");
        Console.SetCursorPosition(Console.WindowWidth - 18, tableY + 1);
        Console.Write("Cantidad");
        Console.ResetColor();

        var products = _inventoryManager.Products;
        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];
            Console.SetCursorPosition(contentX + 2, tableY + 3 + i);

            Console.BackgroundColor = (i == _selectedIndex) ? ConsoleColor.DarkRed : ConsoleColor.Black;

            Console.Write(product.Name.PadRight(Console.WindowWidth - contentX - 25));
            Console.SetCursorPosition(Console.WindowWidth - 20, tableY + 3 + i);
            Console.Write(product.Quantity.ToString().PadLeft(5));
            Console.Write("   [ X ]");
            Console.ResetColor();
        }
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
        var products = _inventoryManager.Products;
        if (products.Count == 0)
        {
            if (key.Key == ConsoleKey.Escape) return new MainMenuView(_inventoryManager);
            return this;
        }

        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                _selectedIndex = (_selectedIndex > 0) ? _selectedIndex - 1 : products.Count - 1;
                break;
            case ConsoleKey.DownArrow:
                _selectedIndex = (_selectedIndex < products.Count - 1) ? _selectedIndex + 1 : 0;
                break;
            case ConsoleKey.Enter or ConsoleKey.Delete or ConsoleKey.X:
                if (products.Count > 0)
                {
                    _inventoryManager.DeleteProduct(products[_selectedIndex].Id);
                    if (_selectedIndex >= _inventoryManager.Products.Count && _inventoryManager.Products.Count > 0)
                    {
                        _selectedIndex = _inventoryManager.Products.Count - 1;
                    }
                }
                break;
            case ConsoleKey.Escape:
                return new MainMenuView(_inventoryManager);
        }
        return this;
    }
}


