using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista para buscar y eliminar productos, con paso de confirmación.
/// </summary>
public class DeleteProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly SideBar _sideBar;
    private readonly SearchField _searchField;

    private readonly List<Label> _tableHeaders;
    private readonly Table _productTable;

    private ConfirmationDialog? _confirmationDialog; // Puede ser null

    private FocusState _focus = FocusState.Content;
    private bool _isConfirmingDelete = false;
    private int _navigationIndex = 4; // Índice de "Eliminar producto"

    public DeleteProductView(InventoryManager manager, int lastNavIndex = 4)
    {
        _inventoryManager = manager;
        _navigationIndex = lastNavIndex;

        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Eliminar producto") { SelectedIndex = _navigationIndex };
        _searchField = new SearchField(27, 6, Console.WindowWidth - 29);

        _tableHeaders = new List<Label>();
        _productTable = new Table(27, 11, Console.WindowWidth - 29, Console.WindowHeight - 13);

        // Varibles de coordenadas para los headers de la tabla
        int headerAreaX = 27;
        int headerAreaY = 11;

        // Color headers
        ConsoleColor colorHeader = ConsoleColor.Yellow;

        // List headers
        _tableHeaders.Add(new Label(headerAreaX + 2, headerAreaY + 1, "Producto", colorHeader));
        _tableHeaders.Add(new Label(headerAreaX + 27, headerAreaY + 1, "SKU", colorHeader));
        _tableHeaders.Add(new Label(headerAreaX + 42, headerAreaY + 1, "Precio", colorHeader));
        _tableHeaders.Add(new Label(headerAreaX + 54, headerAreaY + 1, "Cantidad", colorHeader));

        // Establecer los headers de la tabla de productos
        _productTable.SetHeaders(_tableHeaders);

        UpdateTableContent();
        UpdateFocus();
    }

    private void UpdateFocus()
    {
        _sideBar.HasFocus = _focus == FocusState.Navigation;
        _searchField.HasFocus = _focus == FocusState.Content && !_isConfirmingDelete;
        _productTable.HasFocus = _focus == FocusState.Content && !_isConfirmingDelete;
    }

    private void UpdateTableContent()
    {
        var products = _inventoryManager.SearchProducts(_searchField.Text).ToList();

        _productTable.ClearRows();
        foreach (var p in products)
        {
            _productTable.AddRow(p.Name, p.Quantity.ToString(), "[ X ]");
        }
        _productTable.SelectedIndex = products.Any() ? 0 : -1;
    }

    public void Draw(TuiRenderer renderer)
    {
        new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1).Draw(renderer);
        _sideBar.Draw(renderer);
        new Label(27, 3, "/ Eliminar producto", ConsoleColor.White).Draw(renderer);
        _searchField.Draw(renderer);
        _productTable.Draw(renderer);

        Console.CursorVisible = _searchField.HasFocus;

        // Dibuja el diálogo de confirmación si está activo
        _confirmationDialog?.Draw(renderer);
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        if (_isConfirmingDelete)
        {
            return HandleConfirmationInput(key);
        }

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
            if (_searchField.HandleKey(key))
            {
                UpdateTableContent();
            }
            else
            {
                _productTable.HandleInput(key);
                if (key.Key == ConsoleKey.Enter && _productTable.SelectedIndex != -1)
                {
                    var products = _inventoryManager.SearchProducts(_searchField.Text).ToList();
                    var productToDelete = products[_productTable.SelectedIndex];
                    _confirmationDialog = new ConfirmationDialog(
                        "Confirmar Eliminación",
                        $"¿Seguro que desea eliminar '{productToDelete.Name}'?",
                        "[S]í / [N]o"
                    );
                    _isConfirmingDelete = true;
                }
            }
        }

        return this;
    }

    private IView? HandleConfirmationInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.S)
        {
            var products = _inventoryManager.SearchProducts(_searchField.Text).ToList();
            if (_productTable.SelectedIndex < products.Count)
            {
                var productToDelete = products[_productTable.SelectedIndex];
                _inventoryManager.DeleteProduct(productToDelete.Id);
                UpdateTableContent();
            }
            _isConfirmingDelete = false;
            _confirmationDialog = null; // Se elimina el diálogo
        }
        else if (key.Key is ConsoleKey.N or ConsoleKey.Escape)
        {
            _isConfirmingDelete = false;
            _confirmationDialog = null;
        }
        return this;
    }
}


