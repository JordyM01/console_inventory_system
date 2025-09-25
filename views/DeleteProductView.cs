using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista para buscar y eliminar productos, con paso de confirmación.
/// (Refactorizada para usar el componente Table reutilizable).
/// </summary>
public class DeleteProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly SideBar _sideBar;
    private readonly SearchField _searchField;
    private readonly Table _productTable; // <-- El componente que ahora hace todo el trabajo de la tabla

    private ConfirmationDialog? _confirmationDialog; // Puede ser null

    private FocusState _focus = FocusState.Content;
    private bool _isConfirmingDelete = false;
    private int _navigationIndex = 4; // Índice de "Eliminar producto"

    public DeleteProductView(InventoryManager manager, int lastNavIndex = 4)
    {
        _inventoryManager = manager;
        _navigationIndex = lastNavIndex;

        // --- Inicialización de componentes de la UI ---
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Eliminar producto") { SelectedIndex = _navigationIndex };
        _searchField = new SearchField(27, 6, Console.WindowWidth - 29);
        _productTable = new Table(27, 11, Console.WindowWidth - 29, Console.WindowHeight - 13);

        // --- Configuración de la Tabla ---
        // 1. Se definen las columnas que se mostrarán en la tabla.
        _productTable.SetColumns(
            new ColumnDefinition("Producto", 25),
            new ColumnDefinition("SKU", 15),
            new ColumnDefinition("Precio", 12),
            new ColumnDefinition("Cant.", 6),
            new ColumnDefinition("Acción", 10)
        );

        // 2. Se carga el contenido inicial y se actualiza el foco.
        UpdateTableContent();
        UpdateFocus();
    }

    private void UpdateFocus()
    {
        _sideBar.HasFocus = _focus == FocusState.Navigation;
        // El campo de búsqueda y la tabla comparten el foco de contenido.
        // La tabla necesita saber que tiene foco para renderizar la selección.
        _searchField.HasFocus = _focus == FocusState.Content && !_isConfirmingDelete;
        _productTable.HasFocus = _focus == FocusState.Content && !_isConfirmingDelete;
    }

    /// <summary>
    /// Busca productos y actualiza las filas del componente Table.
    /// </summary>
    private void UpdateTableContent()
    {
        var products = _inventoryManager.SearchProducts(_searchField.Text).ToList();

        // Limpia las filas anteriores
        _productTable.ClearRows();

        // Itera sobre los productos y crea un TableRow para cada uno
        foreach (var p in products)
        {
            var row = new TableRow(new[] {
                p.Name,
                p.Sku,
                $"{p.Price:C}", // Formato de moneda
                p.Quantity.ToString(),
                "[ X ]"
            });

            // Se guarda el objeto Product completo en la propiedad Tag de la fila.
            // Esto permite recuperar el producto fácilmente al seleccionar la fila.
            row.Tag = p;
            _productTable.AddRow(row);
        }

        // Selecciona el primer elemento si la lista no está vacía.
        if (products.Any())
        {
            _productTable.SelectedIndex = 0;
        }
    }

    public void Draw(TuiRenderer renderer)
    {
        // El dibujado es más simple, ya que cada componente se dibuja a sí mismo.
        new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1).Draw(renderer);
        _sideBar.Draw(renderer);
        new Label(27, 3, "/ Eliminar producto", ConsoleColor.Red).Draw(renderer);
        _searchField.Draw(renderer);
        _productTable.Draw(renderer); // <-- La tabla ahora se encarga de todo su dibujado interno.

        Console.CursorVisible = _searchField.HasFocus;

        // Dibuja el diálogo de confirmación si está activo
        _confirmationDialog?.Draw(renderer);
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        // Si estamos en el modo de confirmación, la entrada se maneja por separado.
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
            // Si el campo de búsqueda maneja la tecla, actualiza el contenido.
            if (_searchField.HandleKey(key))
            {
                UpdateTableContent();
            }
            else // Si no, la tecla es para la tabla.
            {
                // Se delega toda la lógica de navegación a la tabla.
                _productTable.HandleInput(key);

                // Si se presiona Enter y hay una fila seleccionada...
                if (key.Key == ConsoleKey.Enter && _productTable.GetSelectedRow() != null)
                {
                    // Se recupera el producto desde la propiedad Tag de la fila seleccionada.
                    var selectedRow = _productTable.GetSelectedRow();
                    var productToDelete = selectedRow?.Tag as Product;

                    if (productToDelete != null)
                    {
                        // Se crea el diálogo de confirmación y se activa el modo modal.
                        _confirmationDialog = new ConfirmationDialog(
                            "Confirmar Eliminación",
                            $"¿Seguro que desea eliminar '{productToDelete.Name}'?",
                            "[S]í / [N]o"
                        );
                        _isConfirmingDelete = true;
                    }
                }
            }
        }

        return this;
    }

    private IView? HandleConfirmationInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.S) // Si se confirma
        {
            var selectedRow = _productTable.GetSelectedRow();
            var productToDelete = selectedRow?.Tag as Product; // Recupera el producto

            if (productToDelete != null)
            {
                _inventoryManager.DeleteProduct(productToDelete.Id);
                UpdateTableContent(); // Actualiza la tabla para reflejar la eliminación
            }
        }

        // Si se presiona 'S', 'N' o 'Escape', se sale del modo de confirmación.
        if (key.Key is ConsoleKey.S or ConsoleKey.N or ConsoleKey.Escape)
        {
            _isConfirmingDelete = false;
            _confirmationDialog = null; // Se elimina el diálogo
        }

        UpdateFocus();
        return this;
    }
}

