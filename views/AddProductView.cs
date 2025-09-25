using System;
using System.Linq;

/// <summary>
/// Vista para agregar un nuevo producto.
/// Utiliza el componente reutilizable ProductForm para toda la lógica del formulario.
/// </summary>
public class AddProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly SideBar _sideBar;
    private readonly ProductForm _productForm;

    // Controla si el foco está en el menú de navegación o en el contenido (el formulario).
    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 1; // Índice de "Agregar producto" en el menú

    public AddProductView(InventoryManager manager, int lastNavIndex = 1)
    {
        _inventoryManager = manager;
        _navigationIndex = lastNavIndex;

        // --- Composición de la Vista ---
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Agregar producto") { SelectedIndex = _navigationIndex };
        _productForm = new ProductForm(27, 7, Console.WindowWidth - 30, Console.WindowHeight - 10);

        // --- Configuración de Eventos del Formulario ---

        // Se define la acción a ejecutar cuando el formulario guarda un producto exitosamente.
        _productForm.OnSave = (newProduct) =>
        {
            _inventoryManager.AddProduct(newProduct);
            // Después de guardar, se limpia el formulario para permitir agregar otro producto.
            _productForm.Clear();
            // Se podría mostrar un mensaje de éxito, pero limpiar el formulario es suficiente indicación.
        };

        // Se define la acción a ejecutar cuando el usuario cancela la edición en el formulario (con ESC).
        _productForm.OnCancel = () =>
        {
            _focusState = FocusState.Navigation;
            UpdateFocus();
        };

        // Se establece el foco inicial en el formulario.
        UpdateFocus();
    }

    /// <summary>
    /// Dibuja los componentes de la vista en la pantalla.
    /// </summary>
    public void Draw(TuiRenderer renderer)
    {
        new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1).Draw(renderer);
        _sideBar.Draw(renderer);
        new Label(27, 3, "/ Agregar producto", ConsoleColor.Green).Draw(renderer);

        // El formulario es un componente autocontenido que sabe cómo dibujarse a sí mismo.
        _productForm.Draw(renderer);
    }

    /// <summary>
    /// Maneja la entrada del teclado y la dirige al componente que tiene el foco.
    /// </summary>
    public IView? HandleInput(ConsoleKeyInfo key)
    {
        if (_focusState == FocusState.Navigation)
        {
            _sideBar.HandleInput(key);
            if (key.Key is ConsoleKey.Enter or ConsoleKey.RightArrow)
            {
                var nextView = NavigationHelper.GetViewByIndex(_sideBar.SelectedIndex, _inventoryManager);
                // Si el usuario selecciona "Agregar producto" de nuevo, solo se cambia el foco al formulario.
                if (nextView is AddProductView)
                {
                    _focusState = FocusState.Content;
                    UpdateFocus();
                    return this;
                }
                return nextView; // Navega a una vista diferente.
            }
        }
        else // FocusState.Content
        {
            // Cuando el foco está en el contenido, toda la entrada es manejada por el formulario.
            // La tecla ESC es manejada por el formulario, que invoca el evento OnCancel.
            _productForm.HandleInput(key);
        }
        return this;
    }

    /// <summary>
    /// Actualiza qué componente tiene el foco visual.
    /// </summary>
    private void UpdateFocus()
    {
        _sideBar.HasFocus = (_focusState == FocusState.Navigation);
        _productForm.HasFocus = (_focusState == FocusState.Content);
    }
}


