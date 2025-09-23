using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista para mostrar productos, ahora compuesta por componentes de TUI.
/// </summary>
public class ShowProductsView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly SideBar _sideBar;
    private readonly Label _title;
    private readonly Frame _frame;
    private readonly TextField _searchField;
    private readonly Table _productTable;

    private FocusState _focus = FocusState.Content;

    public ShowProductsView(InventoryManager manager, int lasNavIndex = 2)
    {
        _inventoryManager = manager;

        _frame = new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Mostrar productos");
        _sideBar.SelectedIndex = lasNavIndex;

        _title = new Label(27, 3, "/ Mostrar productos", ConsoleColor.Green);
        _searchField = new TextField(27, 6, Console.WindowWidth - 29);
        _productTable = new Table(27, 11, Console.WindowWidth - 29, Console.WindowHeight - 13);

        UpdateTableContent();
        UpdateFocus();
    }

    private void UpdateFocus()
    {
        _sideBar.HasFocus = _focus == FocusState.Navigation;
        _searchField.HasFocus = _focus == FocusState.Content;
        // La tabla podría tener su propio foco si se quisiera seleccionar items
        _productTable.HasFocus = _focus == FocusState.Content;
    }

    private void UpdateTableContent()
    {
        var products = _inventoryManager.SearchProducts(_searchField.Text);
        _productTable.SetHeaders("Producto", "SKU", "Precio", "Cant.");
        _productTable.ClearRows();
        foreach (var p in products)
        {
            _productTable.AddRow(p.Name, p.Sku, $"{p.Price:C}", p.Quantity.ToString());
        }
    }

    public void Draw(TuiRenderer renderer)
    {
        _frame.Draw(renderer);
        _sideBar.Draw(renderer);
        _title.Draw(renderer);
        _searchField.Draw(renderer);
        _productTable.Draw(renderer);
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.LeftArrow && _focus == FocusState.Content) _focus = FocusState.Navigation;
        else if (key.Key == ConsoleKey.RightArrow && _focus == FocusState.Navigation) _focus = FocusState.Content;

        UpdateFocus();

        if (_focus == FocusState.Navigation)
        {
            _sideBar.HandleInput(key);
            if (key.Key == ConsoleKey.Enter)
            {
                return NavigationHelper.GetViewByIndex(_sideBar.SelectedIndex, _inventoryManager);
            }
        }
        else // FocusState.Content
        {
            // 1. Se guarda el texto de búsqueda *antes* de procesar la tecla.
            string previousSearchTerm = _searchField.Text;

            // 2. Se llama al método con el nombre correcto: 'HandleInput'.
            _searchField.HandleInput(key);

            // 3. Se comprueba si el texto ha cambiado. Si es así, se actualiza la tabla.
            if (_searchField.Text != previousSearchTerm)
            {
                UpdateTableContent();
            }

            // Se pasa la entrada a la tabla para la navegación interna (arriba/abajo).
            _productTable.HandleInput(key);
        }

        return this;
    }
}
