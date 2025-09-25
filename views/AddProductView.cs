using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista para agregar un nuevo producto, con scroll, validación y lógica de guardado.
/// </summary>
public class AddProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly List<(Label Label, TuiComponent Field)> _formItems = new List<(Label, TuiComponent)>();
    private readonly List<TuiComponent> _focusableComponents = new List<TuiComponent>();

    private readonly SideBar _sideBar;
    private int _focusIndex = 1;
    private int _scrollTop = 0;

    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 1;
    private string _statusMessage = "";

    // Referencias directas a los campos para obtener sus valores
    private readonly TextField _idField, _skuField, _productField, _categoryField, _descField;
    private readonly NumericField _qtyField, _minQtyField, _priceField;
    private readonly Button _cancelButton, _saveButton;

    public AddProductView(InventoryManager manager, int lastNavIndex = 1)
    {
        _inventoryManager = manager;
        _navigationIndex = lastNavIndex;

        // --- Composición de la Vista ---
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Agregar producto") { SelectedIndex = _navigationIndex };
        _focusableComponents.Add(_sideBar);

        // Se instancian todos los componentes visuales
        _idField = new TextField(45, 7, 25) { Text = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(), IsEditable = false };
        _skuField = new TextField(45, 10, 40);
        _productField = new TextField(45, 13, 40);
        _qtyField = new NumericField(45, 16, 20);
        _categoryField = new TextField(45, 19, 40);
        _minQtyField = new NumericField(45, 22, 20);
        _descField = new TextField(45, 25, 40);
        _priceField = new NumericField(45, 28, 20, isCurrency: true);
        _cancelButton = new Button(45, 32, "Cancelar / ESC");
        _saveButton = new Button(68, 32, "Guardar / Enter");

        // Se asocian las etiquetas con sus campos para dibujarlos juntos.
        _formItems.Add((new Label(30, 8, "Id"), _idField));
        _formItems.Add((new Label(30, 11, "SKU"), _skuField));
        _formItems.Add((new Label(30, 14, "Producto"), _productField));
        _formItems.Add((new Label(30, 17, "Cantidad"), _qtyField));
        _formItems.Add((new Label(30, 20, "Categoría"), _categoryField));
        _formItems.Add((new Label(30, 23, "Cant Mínima"), _minQtyField));
        _formItems.Add((new Label(30, 26, "Descripción"), _descField));
        _formItems.Add((new Label(30, 29, "Precio"), _priceField));
        _formItems.Add((new Label(0, 0, ""), _cancelButton));
        _formItems.Add((new Label(0, 0, ""), _saveButton));

        // Se añaden los campos (sin el Id) a la lista de enfocables
        _focusableComponents.AddRange(_formItems.Where(item => item.Field != _idField).Select(item => item.Field));

        UpdateFocus();
    }

    private void UpdateFocus()
    {
        for (int i = 0; i < _focusableComponents.Count; i++)
        {
            _focusableComponents[i].HasFocus = (i == _focusIndex);
        }
        _sideBar.HasFocus = _focusState == FocusState.Navigation;
    }

    public void Draw(TuiRenderer renderer)
    {
        new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1).Draw(renderer);
        _sideBar.Draw(renderer);
        new Label(27, 4, "/ Agregar producto", ConsoleColor.White).Draw(renderer);

        foreach (var item in _formItems)
        {
            // Se accede a los elementos de la tupla por Item1 y Item2.
            int originalY = item.Field.Y;
            int scrolledY = originalY - (_scrollTop * 3);

            if (scrolledY > 5 && scrolledY < Console.WindowHeight - 5)
            {
                item.Field.Y = scrolledY;
                item.Item1.Y = scrolledY + 1; // Item1 es la Label
                item.Field.Draw(renderer);
                item.Item1.Draw(renderer);
                item.Field.Y = originalY;
            }
        }

        if (!string.IsNullOrEmpty(_statusMessage))
            renderer.Write(27, Console.WindowHeight - 3, _statusMessage, ConsoleColor.Red);

        var focused = _focusableComponents[_focusIndex];
        Console.CursorVisible = _focusState == FocusState.Content && focused is TextField tf && tf.IsEditable || focused is NumericField;
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        // --- Lógica de cambio de foco y navegación del menú ---
        if (_focusState == FocusState.Navigation)
        {
            if (key.Key is ConsoleKey.Enter or ConsoleKey.RightArrow)
            {
                var nextView = NavigationHelper.GetViewByIndex(_navigationIndex, _inventoryManager);
                if (nextView is AddProductView) { _focusState = FocusState.Content; return this; }
                return nextView;
            }
            NavigationHelper.HandleMenuNavigation(key, ref _navigationIndex);
            return this;
        }

        // --- Lógica de Navegación y Acciones del Formulario ---
        var focusedComponent = _focusableComponents[_focusIndex];

        if (key.Key == ConsoleKey.Enter && !(focusedComponent is Button))
        {
            EnterEditMode(_focusIndex);
        }
        else
        {
            focusedComponent.HandleInput(key);
        }

        if (key.Key is ConsoleKey.DownArrow or ConsoleKey.Tab)
        {
            _focusIndex = (_focusIndex < _focusableComponents.Count - 1) ? _focusIndex + 1 : 1;
        }
        else if (key.Key is ConsoleKey.UpArrow)
        {
            _focusIndex = (_focusIndex > 1) ? _focusIndex - 1 : _focusableComponents.Count - 1;
        }
        else if (key.Key is ConsoleKey.LeftArrow or ConsoleKey.Escape)
        {
            _focusState = FocusState.Navigation;
        }

        // Lógica de scroll
        int maxVisibleFields = (Console.WindowHeight - 15) / 3;
        if (_focusIndex - 1 > _scrollTop + maxVisibleFields) _scrollTop++;
        if (_focusIndex - 1 < _scrollTop) _scrollTop--;

        UpdateFocus();

        if (focusedComponent == _saveButton && key.Key == ConsoleKey.Enter) return ValidateAndSave();
        if (focusedComponent == _cancelButton && key.Key == ConsoleKey.Enter) return new MainMenuView(_inventoryManager);

        return this;
    }

    private void EnterEditMode(int fieldIndex)
    {
        var focusedComponent = _focusableComponents[fieldIndex];

        var (newValue, result) = ("", EditResult.Canceled);

        if (focusedComponent is TextField tf && tf.IsEditable)
        {
            var inputHandler = new InputHandler(tf.X + 2, tf.Y - (_scrollTop * 3) + 1, tf.Width - 4, tf.Text, InputType.Text);
            (newValue, result) = inputHandler.ProcessInput();
            if (result != EditResult.Canceled) tf.Text = newValue;
        }
        else if (focusedComponent is NumericField nf)
        {
            var inputHandler = new InputHandler(nf.X + 6, nf.Y - (_scrollTop * 3) + 1, nf.Width - 12, nf.Value.ToString(), nf.IsCurrency ? InputType.Decimal : InputType.Integer);
            (newValue, result) = inputHandler.ProcessInput();
            if (result != EditResult.Canceled)
            {
                decimal.TryParse(newValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal parsedValue);
                nf.Value = parsedValue;
            }
        }

        // Si se cancela la edición, el foco vuelve al menú lateral.
        if (result == EditResult.Canceled)
        {
            _focusState = FocusState.Navigation;
            UpdateFocus();
        }
    }

    private IView ValidateAndSave()
    {
        // Resetea el estado de validación
        _statusMessage = "";
        _skuField.State = _productField.State = _qtyField.State = _priceField.State = ValidationState.Pristine;

        // Realiza las validaciones
        bool isValid = true;
        if (string.IsNullOrWhiteSpace(_skuField.Text)) { _skuField.State = ValidationState.Invalid; isValid = false; }
        if (string.IsNullOrWhiteSpace(_productField.Text)) { _productField.State = ValidationState.Invalid; isValid = false; }
        if (_qtyField.Value <= 0) { _qtyField.State = ValidationState.Invalid; isValid = false; }
        if (_priceField.Value <= 0) { _priceField.State = ValidationState.Invalid; isValid = false; }

        if (!isValid)
        {
            _statusMessage = "Error: Por favor, corrija los campos marcados en rojo.";
            return this;
        }

        // Si es válido, crea el producto y lo guarda
        var newProduct = new Product(
            Id: _idField.Text, Sku: _skuField.Text, Name: _productField.Text, Quantity: (int)_qtyField.Value,
            Category: _categoryField.Text, MinQuantity: (int)_minQtyField.Value,
            Description: _descField.Text, Price: _priceField.Value
        );
        _inventoryManager.AddProduct(newProduct);

        // Limpia el formulario para el siguiente producto
        _skuField.Clear(); _productField.Clear(); _qtyField.Clear(); _categoryField.Clear();
        _minQtyField.Clear(); _descField.Clear(); _priceField.Clear();
        _idField.Text = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        _statusMessage = "Producto guardado exitosamente!";
        _focusIndex = 1; // Vuelve el foco al primer campo
        return this;
    }
}


