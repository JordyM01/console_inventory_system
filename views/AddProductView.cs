using static UiComponents;

public class AddProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private Product _newProduct;
    private int _currentFieldIndex = 0;
    private readonly string[] _fieldLabels = { "SKU", "Producto", "Cantidad", "Categoria", "Cant minima", "Descripcion", "Precio" };

    private bool _isEditing = false;
    private string _validationError = "";
    private bool _awaitingG_ForSave = false;

    public AddProductView(InventoryManager manager)
    {
        _inventoryManager = manager;
        _newProduct = new Product(
            Id: Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(), Sku: "", Name: "",
            Quantity: 1, Category: "", MinQuantity: 0, Description: "", Price: 0m
        );
    }

    public void Draw()
    {
        DrawLayout("Agregar producto");
        int contentX = 27, contentY = 3;
        Console.SetCursorPosition(contentX, contentY); Console.Write("/ Agregar producto");

        DrawFormField(contentX, 7, "Id", _newProduct.Id, isFocused: false);
        DrawFormField(contentX, 9, _fieldLabels[0], _newProduct.Sku, _currentFieldIndex == 0 && !_isEditing);
        DrawFormField(contentX, 11, _fieldLabels[1], _newProduct.Name, _currentFieldIndex == 1 && !_isEditing);
        DrawNumericFormField(contentX, 13, _fieldLabels[2], _newProduct.Quantity, _currentFieldIndex == 2 && !_isEditing);
        DrawFormField(contentX, 15, _fieldLabels[3], _newProduct.Category, _currentFieldIndex == 3 && !_isEditing);
        DrawNumericFormField(contentX, 17, _fieldLabels[4], _newProduct.MinQuantity, _currentFieldIndex == 4 && !_isEditing);
        DrawFormField(contentX, 19, _fieldLabels[5], _newProduct.Description, _currentFieldIndex == 5 && !_isEditing);
        DrawNumericFormField(contentX, 21, _fieldLabels[6], _newProduct.Price, _currentFieldIndex == 6 && !_isEditing, isCurrency: true);

        string buttons = "[Enter/Tab] Editar | [↑/↓] Navegar | [Space]+[G] Guardar | [ESC] Salir";
        Console.SetCursorPosition(contentX, 25);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(buttons);

        if (!string.IsNullOrEmpty(_validationError))
        {
            Console.SetCursorPosition(contentX, 27);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(_validationError);
        }
        Console.ResetColor();
    }

    // --- MÉTODO HandleInput COMPLETAMENTE REESCRITO ---
    public IView HandleInput(ConsoleKeyInfo key)
    {
        if (_isEditing) return this;

        // --- Lógica de Guardado ---
        if (_awaitingG_ForSave)
        {
            if (key.Key == ConsoleKey.G)
            {
                _awaitingG_ForSave = false;
                return ValidateAndSave();
            }
            _awaitingG_ForSave = false;
        }
        if (key.Key == ConsoleKey.Spacebar)
        {
            _awaitingG_ForSave = true;
            _validationError = "";
            return this;
        }

        // --- Lógica de Navegación ---
        switch (key.Key)
        {
            // 1. Prioridad: Navegación interna del formulario
            case ConsoleKey.DownArrow:
            case ConsoleKey.Tab:
                _currentFieldIndex = (_currentFieldIndex + 1) % _fieldLabels.Length;
                break;
            case ConsoleKey.UpArrow:
                _currentFieldIndex = (_currentFieldIndex - 1 + _fieldLabels.Length) % _fieldLabels.Length;
                break;

            // 2. Acción de entrar en modo edición
            case ConsoleKey.Enter:
                EnterEditMode(_currentFieldIndex);
                break;

            // 3. Navegación para salir de la vista
            case ConsoleKey.Escape:
            case ConsoleKey.Backspace:
                return new MainMenuView(_inventoryManager);
        }
        return this;
    }

    private void EnterEditMode(int startingFieldIndex)
    {
        _isEditing = true;
        _currentFieldIndex = startingFieldIndex;

        while (true)
        {
            Draw();
            var (newValue, result) = EditField(_currentFieldIndex);
            UpdateProductField(_currentFieldIndex, newValue);

            if (result == EditResult.TabbedForward)
            {
                _currentFieldIndex = (_currentFieldIndex + 1) % _fieldLabels.Length;
                continue;
            }

            if (result == EditResult.Confirmed)
            {
                _currentFieldIndex = (_currentFieldIndex + 1) % _fieldLabels.Length;
            }
            break;
        }

        _isEditing = false;
        Console.CursorVisible = false;
        Draw();
    }

    private (string, EditResult) EditField(int fieldIndex)
    {
        int textInputX = 27 + 18, textInputWidth = 26;
        int numInputX = 27 + 21, numInputWidth = 13;

        return fieldIndex switch
        {
            0 => new InputHandler(textInputX, 9, textInputWidth, _newProduct.Sku, InputType.Text).ProcessInput(),
            1 => new InputHandler(textInputX, 11, textInputWidth, _newProduct.Name, InputType.Text).ProcessInput(),
            2 => new InputHandler(numInputX, 13, numInputWidth, _newProduct.Quantity.ToString(), InputType.Integer).ProcessInput(),
            3 => new InputHandler(textInputX, 15, textInputWidth, _newProduct.Category, InputType.Text).ProcessInput(),
            4 => new InputHandler(numInputX, 17, numInputWidth, _newProduct.MinQuantity.ToString(), InputType.Integer).ProcessInput(),
            5 => new InputHandler(textInputX, 19, textInputWidth, _newProduct.Description, InputType.Text).ProcessInput(),
            6 => new InputHandler(numInputX, 21, numInputWidth, _newProduct.Price.ToString(), InputType.Decimal).ProcessInput(),
            _ => ("", EditResult.Canceled)
        };
    }

    private void UpdateProductField(int fieldIndex, string value)
    {
        switch (fieldIndex)
        {
            case 0: _newProduct = _newProduct with { Sku = value }; break;
            case 1: _newProduct = _newProduct with { Name = value }; break;
            case 2: int.TryParse(value, out int qty); if (qty >= 0) _newProduct = _newProduct with { Quantity = qty }; break;
            case 3: _newProduct = _newProduct with { Category = value }; break;
            case 4: int.TryParse(value, out int minQty); if (minQty >= 0) _newProduct = _newProduct with { MinQuantity = minQty }; break;
            case 5: _newProduct = _newProduct with { Description = value }; break;
            case 6: decimal.TryParse(value, out decimal price); if (price >= 0) _newProduct = _newProduct with { Price = price }; break;
        }
    }

    private IView ValidateAndSave()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(_newProduct.Sku)) errors.Add("SKU es obligatorio.");
        if (string.IsNullOrWhiteSpace(_newProduct.Name)) errors.Add("Producto es obligatorio.");
        if (_newProduct.Quantity <= 0) errors.Add("Cantidad debe ser mayor a 0.");
        if (_newProduct.Price <= 0) errors.Add("Precio debe ser mayor a 0.");

        if (errors.Count > 0)
        {
            _validationError = "Error de validación: " + string.Join(" ", errors);
            return this;
        }

        _inventoryManager.AddProduct(_newProduct);
        return new ShowProductsView(_inventoryManager);
    }
}

