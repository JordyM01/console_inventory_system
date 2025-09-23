using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista para agregar un nuevo producto, compuesta por componentes de TUI.
/// </summary>
public class AddProductView : IView
{
    private readonly SideBar _sideBar;
    private readonly Frame _frame;
    private readonly Label _title;
    private int _focusIndex = 1;

    private readonly InventoryManager _inventoryManager;
    private readonly List<TuiComponent> _components = new List<TuiComponent>();
    private readonly List<TuiComponent> _focusableComponents = new List<TuiComponent>();

    public AddProductView(InventoryManager manager, int lastNavIndex = 1)
    {
        _inventoryManager = manager;

        // --- Composición de la Vista ---
        // Se instancian todos los componentes visuales
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Agregar producto");
        _sideBar.SelectedIndex = lastNavIndex;

        _frame = new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);
        _title = new Label(27, 4, "/ Agregar producto", ConsoleColor.Green);

        // Etiquetas
        _components.Add(new Label(30, 7, "Id"));
        _components.Add(new Label(30, 10, "SKU"));
        _components.Add(new Label(30, 13, "Producto"));
        _components.Add(new Label(30, 16, "Cantidad"));
        _components.Add(new Label(30, 19, "Categoria"));
        _components.Add(new Label(30, 22, "Cant minima"));
        _components.Add(new Label(30, 25, "Descripcion"));
        _components.Add(new Label(30, 28, "Precio"));

        // Campos de entrada
        var idField = new TextField(45, 6, 25) { Text = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper() };
        var skuField = new TextField(45, 9, 40);
        var productField = new TextField(45, 12, 40);
        var qtyField = new NumericField(45, 15, 20);
        var categoryField = new TextField(45, 18, 40);
        var minQtyField = new NumericField(45, 21, 20);
        var descField = new TextField(45, 24, 40);
        var priceField = new NumericField(45, 27, 20, isCurrency: true);

        // Botones
        var cancelButton = new Button(45, 31, "Cancelar / ESC");
        var saveButton = new Button(68, 31, "Guardar / CTRL+S");

        // Lista de todos los componentes para dibujarlos fácilmente
        _components.AddRange(new TuiComponent[] {
            _frame, _sideBar, _title, idField, skuField, productField, qtyField,
            categoryField, minQtyField, descField, priceField, cancelButton, saveButton
        });

        // Lista de componentes que pueden recibir el foco del usuario
        _focusableComponents.AddRange(new TuiComponent[] {
            _sideBar, skuField, productField, qtyField, categoryField,
            minQtyField, descField, priceField, cancelButton, saveButton
        });

        UpdateFocus();
    }

    private void UpdateFocus()
    {
        for (int i = 0; i < _focusableComponents.Count; i++)
        {
            _focusableComponents[i].HasFocus = (i == _focusIndex);
        }
    }

    public void Draw(TuiRenderer renderer)
    {
        foreach (var component in _components)
        {
            component.Draw(renderer);
        }
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        // Navegación principal entre el menú lateral y el contenido
        if (key.Key == ConsoleKey.LeftArrow) _focusIndex = 0;
        if (key.Key == ConsoleKey.RightArrow && _focusIndex == 0) _focusIndex = 1;

        // Navegación vertical entre los campos del formulario
        if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.Tab)
        {
            if (_focusIndex > 0 && _focusIndex < _focusableComponents.Count - 1) _focusIndex++;
        }
        if (key.Key == ConsoleKey.UpArrow)
        {
            if (_focusIndex > 1) _focusIndex--;
        }

        UpdateFocus();

        // Pasa la entrada de teclado al componente que tiene el foco
        var focusedComponent = _focusableComponents[_focusIndex];
        focusedComponent.HandleInput(key);

        // Lógica de navegación al presionar Enter en el menú lateral
        if (focusedComponent is SideBar sidebar && key.Key == ConsoleKey.Enter)
        {
            return NavigationHelper.GetViewByIndex(sidebar.SelectedIndex, _inventoryManager);
        }

        // Aquí debe ir la lógica para guardar el producto al presionar Enter en el botón de guardar
        // if (focusedComponent == saveButton && key.Key == ConsoleKey.Enter) { ... }

        return this;
    }
}


