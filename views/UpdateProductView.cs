using System;
using System.Linq;

/// <summary>
/// Vista para buscar, actualizar cantidad y editar detalles de productos.
/// Combina una tabla de productos con un formulario modal para edición detallada.
/// </summary>
public class UpdateProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly SideBar _sideBar;
    private readonly SearchField _searchField;
    private readonly Table _productTable;

    // --- Estado de la Vista Principal ---
    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 3;

    /// <summary>
    /// El formulario de edición. Es no nulo solo cuando el modo de edición está activo.
    /// </summary>
    private ProductForm? _editForm;

    public UpdateProductView(InventoryManager manager, int lastNavIndex = 3)
    {
        _inventoryManager = manager;
        _navigationIndex = lastNavIndex;

        // --- Inicialización de Componentes Principales ---
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Actualizar producto") { SelectedIndex = _navigationIndex };
        _searchField = new SearchField(27, 6, Console.WindowWidth - 29);
        _productTable = new Table(27, 11, Console.WindowWidth - 29, Console.WindowHeight - 13);

        _productTable.SetColumns(
            new ColumnDefinition("Producto", 40),
            new ColumnDefinition("Cantidad", 20)
        );

        UpdateTableContent();
        UpdateFocus();
    }

    /// <summary>
    /// Recarga los datos de los productos desde el InventoryManager,
    /// los filtra según el campo de búsqueda y actualiza las filas de la tabla.
    /// </summary>
    private void UpdateTableContent()
    {
        int previousIndex = _productTable.SelectedIndex;
        var products = _inventoryManager.SearchProducts(_searchField.Text).ToList();
        _productTable.ClearRows();

        foreach (var p in products)
        {
            string quantityDisplay = $"[ - ]   {p.Quantity.ToString().PadLeft(5)}   [ + ]";
            var row = new TableRow(new[] { p.Name, quantityDisplay });
            row.Tag = p;
            _productTable.AddRow(row);
        }
        if (products.Any())
        {
            _productTable.SelectedIndex = Math.Min(Math.Max(0, previousIndex), products.Count - 1);
        }
    }

    /// <summary>
    /// Dibuja la vista en la consola. Delega al formulario de edición si está activo.
    /// </summary>
    public void Draw(TuiRenderer renderer)
    {
        DrawMainView(renderer);
        // Si el formulario de edición está activo, lo dibuja encima de la vista principal.
        _editForm?.Draw(renderer);
    }

    /// <summary>
    /// Dibuja los componentes de la vista principal (tabla, búsqueda, menú lateral).
    /// </summary>
    private void DrawMainView(TuiRenderer renderer)
    {
        new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1).Draw(renderer);
        _sideBar.Draw(renderer);
        new Label(27, 3, "/ Actualizar producto", ConsoleColor.Cyan).Draw(renderer);
        _searchField.Draw(renderer);

        // Solo dibuja la tabla si el formulario de edición NO está activo.
        if (_editForm == null)
        {
            _productTable.Draw(renderer);
            Console.CursorVisible = _searchField.HasFocus;
        }
        else
        {
            Console.CursorVisible = false;
        }
    }

    /// <summary>
    /// Maneja la entrada del teclado. Si el formulario de edición está activo, le pasa el control.
    /// </summary>
    public IView? HandleInput(ConsoleKeyInfo key)
    {
        if (_editForm != null)
        {
            // Cuando el formulario está activo, solo él maneja el input.
            _editForm.HandleInput(key);
            // El formulario se encarga de cerrarse a través de los eventos OnSave/OnCancel.
            return this;
        }

        if (_focusState == FocusState.Navigation)
        {
            _sideBar.HandleInput(key);
            if (key.Key is ConsoleKey.Enter or ConsoleKey.RightArrow)
            {
                var nextView = NavigationHelper.GetViewByIndex(_sideBar.SelectedIndex, _inventoryManager);
                if (nextView is UpdateProductView) { _focusState = FocusState.Content; } else { return nextView; }
            }
        }
        else // FocusState.Content
        {
            var selectedRow = _productTable.GetSelectedRow();
            var product = selectedRow?.Tag as Product;

            // --- LÓGICA DE MANEJO DE INPUT REFACTORIZADA ---
            switch (key.Key)
            {
                // Navegación al Sidebar
                case ConsoleKey.Escape:
                case ConsoleKey.LeftArrow:
                    _focusState = FocusState.Navigation;
                    break;

                // Acción de Enter siempre abre el modal
                case ConsoleKey.Enter:
                    HandleEnterAction();
                    break;

                // Modificación rápida de Cantidad
                case ConsoleKey.OemPlus:
                case ConsoleKey.Add:
                    if (product != null)
                    {
                        _inventoryManager.UpdateProductQuantity(product.Id, 1);
                        UpdateTableContent();
                    }
                    break;
                case ConsoleKey.OemMinus:
                case ConsoleKey.Subtract:
                    if (product != null)
                    {
                        _inventoryManager.UpdateProductQuantity(product.Id, -1);
                        UpdateTableContent();
                    }
                    break;

                // Búsqueda o navegación de tabla
                default:
                    // La flecha derecha no hace nada y se ignora
                    if (key.Key == ConsoleKey.RightArrow) break;

                    if (_searchField.HandleKey(key))
                    {
                        UpdateTableContent();
                    }
                    else
                    {
                        // Se pasa la entrada a la tabla para el movimiento de arriba/abajo
                        _productTable.HandleInput(key);
                    }
                    break;
            }
        }
        UpdateFocus();
        return this;
    }

    /// <summary>
    /// Determina qué acción realizar al presionar Enter en la tabla principal.
    /// Ahora siempre abre el modal de edición para el producto seleccionado.
    /// </summary>
    private void HandleEnterAction()
    {
        var selectedRow = _productTable.GetSelectedRow();
        if (selectedRow?.Tag is not Product product) return;

        // Activa el modo de edición creando una instancia del formulario en el área de contenido
        _editForm = new ProductForm(27, 7, Console.WindowWidth - 30, Console.WindowHeight - 10);
        _editForm.LoadProduct(product);

        // Define qué hacer cuando el formulario se guarda
        _editForm.OnSave = (updatedProduct) =>
        {
            _inventoryManager.UpdateProduct(updatedProduct);
            _editForm.HasFocus = false;
            _editForm = null; // Cierra el formulario
            UpdateTableContent();
            UpdateFocus();
        };
        // Define qué hacer cuando el formulario se cancela
        _editForm.OnCancel = () =>
        {
            _editForm.HasFocus = false;
            _editForm = null; // Cierra el formulario
            UpdateFocus();
        };
        UpdateFocus();
    }

    /// <summary>
    /// Actualiza el estado de foco de los componentes basado en si el modo de edición está activo.
    /// </summary>
    private void UpdateFocus()
    {
        bool isEditing = _editForm != null;
        _sideBar.HasFocus = _focusState == FocusState.Navigation && !isEditing;
        _searchField.HasFocus = _focusState == FocusState.Content && !isEditing;
        _productTable.HasFocus = _focusState == FocusState.Content && !isEditing;

        // Si el formulario está activo, le damos el foco.
        if (_editForm != null)
        {
            _editForm.HasFocus = true;
        }
    }
}

