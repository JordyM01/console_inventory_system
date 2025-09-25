using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista para buscar, mostrar y ver detalles de productos.
/// </summary>
public class ShowProductsView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly SearchField _searchField;
    private readonly SideBar _sideBar;

    private List<Product> _filteredProducts;
    private int _selectedIndex = -1;
    private int _scrollTop = 0; // Indice del primer producto visible en la tabla

    // Estado para controlar el foco entre el menu y el contenido
    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 2; // Indice de vista Mostrar productos

    // Estado para controlar si se muestra la tabla o los detalles
    private bool _isViewingDetails = false;

    public ShowProductsView(InventoryManager manager, int lastNavIndex = 2)
    {
        _inventoryManager = manager;
        _filteredProducts = _inventoryManager.Products.ToList();

        _navigationIndex = lastNavIndex;
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Mostrar productos") { SelectedIndex = _navigationIndex };
        _searchField = new SearchField(27, 6, Console.WindowWidth - 29);

        UpdateFocus();
    }

    /// <summary>
    /// Actualiza qué componente tiene el foco visual.
    /// </summary>
    private void UpdateFocus()
    {
        _sideBar.HasFocus = _focusState == FocusState.Navigation;
        _searchField.HasFocus = _focusState == FocusState.Content;
    }

    private void UpdateFilteredList()
    {
        _filteredProducts = _inventoryManager.SearchProducts(_searchField.Text).ToList();
        _selectedIndex = _filteredProducts.Any() ? 0 : -1; // Selecciona el primero si hay resultados
        _scrollTop = 0;
    }

    public void Draw(TuiRenderer renderer)
    {
        // --- DIBUJADO DE COMPONENTES ESTÁTICOS ---
        new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1).Draw(renderer);
        // Se crea un SideBar temporalmente para el dibujado
        _sideBar.Draw(renderer);
        new Label(27, 3, "/ Mostrar productos", ConsoleColor.Green).Draw(renderer);

        _searchField.Draw(renderer);

        if (!_searchField.HasFocus)
        {
            Console.CursorVisible = false;
        }

        // --- DIBUJADO CONDICIONAL: TABLA O DETALLES ---
        int contentAreaX = 27;
        int contentAreaY = 11;
        int contentAreaWidth = Console.WindowWidth - 29;
        int contentAreaHeight = Console.WindowHeight - 13;

        if (_isViewingDetails && _selectedIndex > -1)
        {
            var product = _filteredProducts[_selectedIndex];
            DrawProductDetails(renderer, product, contentAreaX, contentAreaY, contentAreaWidth, contentAreaHeight);
        }
        else
        {
            DrawProductTable(renderer, contentAreaX, contentAreaY, contentAreaWidth, contentAreaHeight);
        }
    }

    private void DrawProductTable(TuiRenderer renderer, int x, int y, int width, int height)
    {
        new Frame(x, y, width, height).Draw(renderer);

        // Dibuja la cabecera
        renderer.Write(x + 2, y + 1, "Producto".PadRight(25), ConsoleColor.Yellow);
        renderer.Write(x + 27, y + 1, "SKU".PadRight(15), ConsoleColor.Yellow);
        renderer.Write(x + 42, y + 1, "Precio".PadRight(12), ConsoleColor.Yellow);
        renderer.Write(x + 54, y + 1, "Cant.", ConsoleColor.Yellow);

        int availableRows = (height - 3) / 2; // Espacio para el borde y espaciado de línea

        for (int i = 0; i < availableRows; i++)
        {
            int productIndex = _scrollTop + i;
            int currentLineY = y + 3 + (i * 2); // Deja una línea en blanco entre cada producto

            // Limpia la línea antes de dibujar
            renderer.Write(x + 1, currentLineY, new string(' ', width - 2));

            if (productIndex < _filteredProducts.Count)
            {
                var p = _filteredProducts[productIndex];
                var fg = ConsoleColor.White;
                var bg = ConsoleColor.Black;

                if (productIndex == _selectedIndex)
                {
                    fg = ConsoleColor.Black;
                    bg = ConsoleColor.Gray;
                }

                // Dibuja los datos del producto
                renderer.Write(x + 2, currentLineY, p.Name.PadRight(25).Substring(0, 25), fg, bg);
                renderer.Write(x + 27, currentLineY, p.Sku.PadRight(15).Substring(0, 15), fg, bg);
                renderer.Write(x + 42, currentLineY, $"{p.Price:C}".PadRight(12).Substring(0, 12), fg, bg);
                renderer.Write(x + 54, currentLineY, p.Quantity.ToString().PadRight(5), fg, bg);
            }
        }
    }

    private void DrawProductDetails(TuiRenderer renderer, Product p, int x, int y, int width, int height)
    {
        // Limpia el área y dibuja el marco
        renderer.Write(x, y, new string(' ', width * height));
        new Frame(x, y, width, height).Draw(renderer);

        renderer.Write(x + 2, y + 2, $"ID:          {p.Id}");
        renderer.Write(x + 2, y + 3, $"SKU:         {p.Sku}");
        renderer.Write(x + 2, y + 4, $"Producto:    {p.Name}");
        renderer.Write(x + 2, y + 5, $"Categoría:   {p.Category}");
        renderer.Write(x + 2, y + 6, $"Cantidad:    {p.Quantity}");
        renderer.Write(x + 2, y + 7, $"Cant. Mín:   {p.MinQuantity}");
        renderer.Write(x + 2, y + 8, $"Precio:      {p.Price:C}");
        renderer.Write(x + 2, y + 10, $"Descripción: {p.Description}");

        string closeMessage = "[Presione cualquier tecla para volver]";
        renderer.Write(x + (width - closeMessage.Length) / 2, y + height - 2, closeMessage, ConsoleColor.Yellow);
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        if (_isViewingDetails)
        {
            _isViewingDetails = false;
            return this;
        }

        // --- MANEJO DE TECLADO BASADO EN FOCO ---
        if (_focusState == FocusState.Navigation)
        {
            _sideBar.HandleInput(key);
            if (key.Key is ConsoleKey.Enter or ConsoleKey.RightArrow)
            {
                var nextView = NavigationHelper.GetViewByIndex(_sideBar.SelectedIndex, _inventoryManager);
                // Si la vista seleccionada es esta misma, solo cambia el foco.
                if (nextView is ShowProductsView)
                {
                    _focusState = FocusState.Content;
                    UpdateFocus();
                    return this;
                }
                return nextView;
            }
        }
        else // FocusState.Content
        {
            // Pasa el foco al menú lateral
            if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow)
            {
                _focusState = FocusState.Navigation;
                UpdateFocus();
                return this;
            }

            // Pasa la tecla al campo de búsqueda. Si la usa, actualiza la lista.
            if (_searchField.HandleKey(key))
            {
                UpdateFilteredList();
                return this;
            }

            // Si no fue usada por la búsqueda, procesa la navegación de la tabla.
            int availableRows = (Console.WindowHeight - 13 - 3) / 2;
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (_selectedIndex > 0) _selectedIndex--;
                    if (_selectedIndex < _scrollTop) _scrollTop = _selectedIndex;
                    break;
                case ConsoleKey.DownArrow:
                    if (_selectedIndex < _filteredProducts.Count - 1) _selectedIndex++;
                    if (_selectedIndex >= _scrollTop + availableRows) _scrollTop++;
                    break;
                case ConsoleKey.Enter:
                    if (_selectedIndex != -1) _isViewingDetails = true;
                    break;
            }
        }
        return this;
    }
}
