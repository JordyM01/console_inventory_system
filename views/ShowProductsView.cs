using static UiComponents;

/// <summary>
/// Vista para buscar, mostrar y ver detalles de productos.
/// </summary>
public class ShowProductsView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly TextField _searchField;
    private List<Product> _filteredProducts;
    private int _selectedIndex = -1;
    private int _scrollTop = 0;
    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 2;
    private bool _isViewingDetails = false;

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
        UiComponents.DrawLayout("Mostrar productos", _navigationIndex, _focusState);
        int contentX = 27, contentY = 3;
        Console.SetCursorPosition(contentX, contentY);
        Console.Write("/ Mostrar productos");

        int tableY = 11;
        int tableHeight = Console.WindowHeight - tableY - 2;

        if (_isViewingDetails && _selectedIndex > -1 && _selectedIndex < _filteredProducts.Count)
        {
            UiComponents.DrawProductDetailsPanel(
                _filteredProducts[_selectedIndex],
                contentX, tableY, Console.WindowWidth - contentX - 2, tableHeight
            );
        }
        else
        {
            DrawProductTable(contentX, tableY, Console.WindowWidth - contentX - 2, tableHeight);
        }

        _searchField.Draw();
        Console.CursorVisible = _focusState == FocusState.Content && !_isViewingDetails;
    }

    private void DrawProductTable(int x, int y, int width, int height)
    {
        int availableRows = (height - 4) / 2;

        Console.SetCursorPosition(x, y - 2);
        Console.Write(new string(' ', width));

        if (!_filteredProducts.Any())
        {
            string noResults = "No se encontraron resultados para su búsqueda.";
            Console.SetCursorPosition(x, y - 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(noResults);
            Console.ResetColor();
            DrawBox(x, y, width, height);
            return;
        }

        DrawBox(x, y, width, height);
        Console.SetCursorPosition(x + 2, y + 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Producto".PadRight(25));
        Console.Write("SKU".PadRight(15));
        Console.Write("Precio".PadRight(12));
        Console.Write("Cant.");
        Console.ResetColor();

        for (int i = 0; i < availableRows; i++)
        {
            int productIndex = _scrollTop + i;
            int currentLineY = y + 3 + (i * 2);

            Console.SetCursorPosition(x + 2, currentLineY);
            Console.Write(new string(' ', width - 4));
            if (productIndex >= _filteredProducts.Count) continue;

            var product = _filteredProducts[productIndex];
            Console.SetCursorPosition(x + 2, currentLineY);
            if (productIndex == _selectedIndex && _focusState == FocusState.Content)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            string name = product.Name.PadRight(25);
            string sku = product.Sku.PadRight(15);
            string price = $"{product.Price:C}".PadRight(12);
            string qty = product.Quantity.ToString();

            Console.Write(name.Substring(0, Math.Min(name.Length, 25)));
            Console.Write(sku.Substring(0, Math.Min(sku.Length, 15)));
            Console.Write(price.Substring(0, Math.Min(price.Length, 12)));
            Console.Write(qty);
            Console.ResetColor();
        }
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
        if (_isViewingDetails)
        {
            _isViewingDetails = false;
            return this;
        }

        if (_focusState == FocusState.Navigation)
        {
            if (key.Key is ConsoleKey.Enter or ConsoleKey.RightArrow)
            {
                var nextView = NavigationHelper.GetViewByIndex(_navigationIndex, _inventoryManager);
                if (nextView is ShowProductsView) { _focusState = FocusState.Content; return this; }
                return nextView;
            }
            NavigationHelper.HandleMenuNavigation(key, ref _navigationIndex);
            return this;
        }

        if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow)
        {
            _focusState = FocusState.Navigation;
            return this;
        }

        if (key.Key == ConsoleKey.Enter && _selectedIndex != -1)
        {
            _isViewingDetails = true;
            return this;
        }

        if (_searchField.HandleKey(key))
        {
            UpdateFilteredList();
            return this;
        }

        int availableRows = (Console.WindowHeight - 11 - 6) / 2;
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


