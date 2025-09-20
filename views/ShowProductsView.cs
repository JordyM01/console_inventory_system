using static UiComponents;

public class ShowProductsView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly TextField _searchField;
    private List<Product> _filteredProducts;
    private int _selectedIndex = -1;
    private int _scrollTop = 0;

    public ShowProductsView(InventoryManager manager)
    {
        _inventoryManager = manager;
        _filteredProducts = _inventoryManager.Products.ToList();
        _searchField = new TextField(27, 6, Console.WindowWidth - 27 - 3);
    }

    private void UpdateFilteredList()
    {
        _filteredProducts = _inventoryManager.SearchProducts(_searchField.Text).ToList();
        _selectedIndex = -1;
        _scrollTop = 0;
    }

    public void Draw()
    {
        DrawLayout("Mostrar productos");
        int contentX = 27, contentY = 3;

        Console.SetCursorPosition(contentX, contentY);
        Console.Write("/ Buscar producto");

        int tableY = 11;
        int tableHeight = Console.WindowHeight - tableY - 2;
        int availableRows = tableHeight - 4;

        // Limpia el área de mensaje de error en cada redibujado
        Console.SetCursorPosition(contentX, tableY - 2);
        Console.Write(new string(' ', Console.WindowWidth - contentX - 2));

        if (!_filteredProducts.Any())
        {
            string noResults = "No se encontraron resultados para su búsqueda.";
            Console.SetCursorPosition(contentX, tableY - 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(noResults);
            Console.ResetColor();
            DrawBox(contentX, tableY, Console.WindowWidth - contentX - 2, tableHeight);
        }
        else
        {
            DrawBox(contentX, tableY, Console.WindowWidth - contentX - 2, tableHeight);
            Console.SetCursorPosition(contentX + 2, tableY + 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Producto".PadRight(Console.WindowWidth - contentX - 30));
            Console.Write("SKU".PadRight(15));
            Console.Write("Cant.");
            Console.ResetColor();

            for (int i = 0; i < availableRows; i++)
            {
                int productIndex = _scrollTop + i;
                int currentLineY = tableY + 3 + i;

                Console.SetCursorPosition(contentX + 2, currentLineY);
                Console.Write(new string(' ', Console.WindowWidth - contentX - 4));

                if (productIndex >= _filteredProducts.Count) continue;

                var product = _filteredProducts[productIndex];
                Console.SetCursorPosition(contentX + 2, currentLineY);

                if (productIndex == _selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                string name = product.Name.PadRight(Console.WindowWidth - contentX - 30);
                string sku = product.Sku.PadRight(15);
                string qty = product.Quantity.ToString();

                int nameWidth = Console.WindowWidth - contentX - 35;
                Console.Write(name.Substring(0, Math.Min(name.Length, nameWidth)));
                Console.Write(sku.Substring(0, Math.Min(sku.Length, 15)));
                Console.Write(qty);

                Console.ResetColor();
            }
        }

        // --- CORRECCIÓN CLAVE ---
        // Se mueve el dibujado del campo de búsqueda al FINAL del método Draw().
        // Esto asegura que el cursor se posicione correctamente DESPUÉS de que todo lo demás
        // (incluida la tabla) haya sido dibujado.
        _searchField.Draw();
        Console.CursorVisible = true;
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
        if (_searchField.HandleKey(key))
        {
            UpdateFilteredList();
            return this;
        }

        int availableRows = Console.WindowHeight - 11 - 6;
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                if (_filteredProducts.Any())
                {
                    _selectedIndex = _selectedIndex == -1 ? _filteredProducts.Count - 1 : Math.Max(0, _selectedIndex - 1);
                    if (_selectedIndex < _scrollTop) _scrollTop = _selectedIndex;
                }
                break;
            case ConsoleKey.DownArrow:
                if (_filteredProducts.Any())
                {
                    _selectedIndex = _selectedIndex == -1 ? 0 : Math.Min(_filteredProducts.Count - 1, _selectedIndex + 1);
                    if (_selectedIndex >= _scrollTop + availableRows) _scrollTop = _selectedIndex - availableRows + 1;
                }
                break;
            case ConsoleKey.Escape:
                Console.CursorVisible = false;
                return new MainMenuView(_inventoryManager);
        }
        return this;
    }
}
