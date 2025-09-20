using static UiComponents;

public class ShowProductsView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly TextField _searchField;
    private List<Product> _filteredProducts;
    private int _selectedIndex = -1;
    private int _scrollTop = 0;
    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 2;

    public ShowProductsView(InventoryManager manager)
    {
        _inventoryManager = manager;
        _filteredProducts = _inventoryManager.Products.ToList();
        _searchField = new TextField(27, 6, Console.WindowWidth - 27 - 3);
    }

    public void Draw()
    {
        DrawLayout("Mostrar productos", _focusState);
        Console.CursorVisible = _focusState == FocusState.Content;
        int contentX = 27, contentY = 3;
        Console.SetCursorPosition(contentX, contentY);
        Console.Write("/ Buscar producto");

        int tableY = 11;
        int tableHeight = Console.WindowHeight - tableY - 2;
        int availableRows = tableHeight - 4;

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
                if (productIndex == _selectedIndex && _focusState == FocusState.Content)
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
        _searchField.Draw();
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
        if (_focusState == FocusState.Navigation)
        {
            if (key.Key is ConsoleKey.Enter or ConsoleKey.RightArrow)
            {
                var nextView = NavigationHelper.GetViewByIndex(_navigationIndex, _inventoryManager);
                if (nextView is ShowProductsView) { _focusState = FocusState.Content; return this; }
                return nextView;
            }
            // CORRECCIÓN (CS1501): Se elimina el tercer argumento 'manager'.
            NavigationHelper.HandleMenuNavigation(key, ref _navigationIndex);
            return this;
        }

        if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow) { _focusState = FocusState.Navigation; return this; }

        if (_searchField.HandleKey(key))
        {
            _filteredProducts = _inventoryManager.SearchProducts(_searchField.Text).ToList();
            _selectedIndex = -1; _scrollTop = 0;
            return this;
        }

        int availableRows = Console.WindowHeight - 11 - 6;
        if (key.Key == ConsoleKey.UpArrow && _filteredProducts.Any())
        {
            _selectedIndex = _selectedIndex == -1 ? _filteredProducts.Count - 1 : Math.Max(0, _selectedIndex - 1);
            if (_selectedIndex < _scrollTop) _scrollTop = _selectedIndex;
        }
        if (key.Key == ConsoleKey.DownArrow && _filteredProducts.Any())
        {
            _selectedIndex = _selectedIndex == -1 ? 0 : Math.Min(_filteredProducts.Count - 1, _selectedIndex + 1);
            if (_selectedIndex >= _scrollTop + availableRows) _scrollTop = _selectedIndex - availableRows + 1;
        }
        return this;
    }
}


