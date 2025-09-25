using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista para buscar, mostrar y ver detalles de productos.
/// </summary>
public class ShowProductsView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly SideBar _sideBar;
    private readonly SearchField _searchField;
    private readonly Table _productTable; // <-- Usamos el componente Table reutilizable

    // --- Variables de Estado de la Vista ---
    private FocusState _focusState = FocusState.Content;
    private bool _isViewingDetails = false;
    private int _navigationIndex = 2; // Índice de "Mostrar productos"

    public ShowProductsView(InventoryManager manager, int lastNavIndex = 2)
    {
        _inventoryManager = manager;
        _navigationIndex = lastNavIndex;

        // --- Inicialización de Componentes de UI ---
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Mostrar productos") { SelectedIndex = _navigationIndex };
        _searchField = new SearchField(27, 6, Console.WindowWidth - 29);
        _productTable = new Table(27, 11, Console.WindowWidth - 29, Console.WindowHeight - 13);

        // --- Configuración de la Tabla ---
        _productTable.SetColumns(
            new ColumnDefinition("Producto", 25),
            new ColumnDefinition("SKU", 15),
            new ColumnDefinition("Precio", 12),
            new ColumnDefinition("Cant.", 10)
        );

        UpdateTableContent();
        UpdateFocus();
    }

    /// <summary>
    /// Actualiza qué componente tiene el foco visual.
    /// </summary>
    private void UpdateFocus()
    {
        _sideBar.HasFocus = _focusState == FocusState.Navigation;
        _searchField.HasFocus = _focusState == FocusState.Content && !_isViewingDetails;
        _productTable.HasFocus = _focusState == FocusState.Content && !_isViewingDetails;
    }

    /// <summary>
    /// Busca productos basados en el texto del SearchField y puebla la Tabla.
    /// </summary>
    private void UpdateTableContent()
    {
        var products = _inventoryManager.SearchProducts(_searchField.Text).ToList();
        _productTable.ClearRows();

        foreach (var p in products)
        {
            var row = new TableRow(new[] {
                p.Name,
                p.Sku,
                $"{p.Price:C}",
                p.Quantity.ToString()
            });
            row.Tag = p; // Guardamos el objeto Product completo en la fila
            _productTable.AddRow(row);
        }

        if (products.Any())
        {
            _productTable.SelectedIndex = 0;
        }
    }

    public void Draw(TuiRenderer renderer)
    {
        // Dibuja los componentes base
        new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1).Draw(renderer);
        _sideBar.Draw(renderer);
        new Label(27, 3, "/ Mostrar productos", ConsoleColor.Magenta).Draw(renderer);
        _searchField.Draw(renderer);

        Console.CursorVisible = _searchField.HasFocus;

        // --- DIBUJADO CONDICIONAL: TABLA O DETALLES ---
        if (_isViewingDetails)
        {
            var selectedRow = _productTable.GetSelectedRow();
            var product = selectedRow?.Tag as Product;
            if (product != null)
            {
                // Coordenadas del área de contenido
                int contentAreaX = 27;
                int contentAreaY = 11;
                int contentAreaWidth = Console.WindowWidth - 29;
                int contentAreaHeight = Console.WindowHeight - 13;
                DrawProductDetails(renderer, product, contentAreaX, contentAreaY, contentAreaWidth, contentAreaHeight);
            }
        }
        else
        {
            // La tabla se encarga de su propio dibujado
            _productTable.Draw(renderer);
        }
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        // Si estamos viendo detalles, cualquier tecla nos devuelve a la tabla.
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
            if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow)
            {
                _focusState = FocusState.Navigation;
                UpdateFocus();
                return this;
            }

            // Si el campo de búsqueda usa la tecla, actualiza la tabla.
            if (_searchField.HandleKey(key))
            {
                UpdateTableContent();
                return this;
            }

            // Si no, la tecla es para la tabla.
            _productTable.HandleInput(key); // Delegamos la navegación a la tabla

            if (key.Key == ConsoleKey.Enter && _productTable.GetSelectedRow() != null)
            {
                _isViewingDetails = true; // Cambiamos al modo de vista de detalles
            }
        }
        return this;
    }

    /// <summary>
    /// Dibuja el panel de detalles para un producto específico.
    /// </summary>
    private void DrawProductDetails(TuiRenderer renderer, Product p, int x, int y, int width, int height)
    {
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
}
