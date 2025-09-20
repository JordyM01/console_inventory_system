using static UiComponents; // Corregido: Uiomponents -> UiComponents

// --- Vista para Actualizar la Cantidad de Productos ---
public class UpdateProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private int _selectedIndex = 0;

    public UpdateProductView(InventoryManager manager)
    {
        _inventoryManager = manager;
    }

    public void Draw()
    {
        DrawLayout("Actualizar producto");

        int contentX = 27;
        int contentY = 3;

        Console.SetCursorPosition(contentX, contentY);
        Console.Write("/ Actualizar producto");

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

            if (i == _selectedIndex) Console.BackgroundColor = ConsoleColor.DarkCyan;

            Console.Write(product.Name.PadRight(Console.WindowWidth - contentX - 22));
            Console.ResetColor();

            string quantityStr = product.Quantity.ToString();
            Console.SetCursorPosition(Console.WindowWidth - 20, tableY + 3 + i);
            Console.Write("[-] ");
            Console.Write(quantityStr.PadLeft(5));
            Console.Write(" [+]");
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
            case ConsoleKey.OemPlus or ConsoleKey.Add or ConsoleKey.RightArrow:
                _inventoryManager.UpdateProductQuantity(products[_selectedIndex].Id, 1);
                break;
            case ConsoleKey.OemMinus or ConsoleKey.Subtract or ConsoleKey.LeftArrow:
                _inventoryManager.UpdateProductQuantity(products[_selectedIndex].Id, -1);
                break;
            case ConsoleKey.Escape:
                return new MainMenuView(_inventoryManager);
        }
        return this;
    }
}

