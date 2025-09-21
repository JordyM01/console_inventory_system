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
    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 1;

    public AddProductView(InventoryManager manager)
    {
        _inventoryManager = manager;
        _newProduct = new Product(Id: Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(), Sku: "", Name: "", Quantity: 1, Category: "", MinQuantity: 0, Description: "", Price: 0m);
    }

    public void Draw()
    {
        // Se pasa el nombre de la vista ("Agregar producto") a DrawLayout para el marcador ">".
        UiComponents.DrawLayout("Agregar producto", _navigationIndex, _focusState);
        Console.CursorVisible = _focusState == FocusState.Content && _isEditing;
        int contentX = 27, contentY = 3;
        Console.SetCursorPosition(contentX, contentY); Console.Write("/ Agregar producto");
        DrawFormField(contentX, 7, "Id", _newProduct.Id, false);
        DrawFormField(contentX, 9, _fieldLabels[0], _newProduct.Sku, _focusState == FocusState.Content && _currentFieldIndex == 0 && !_isEditing);
        DrawFormField(contentX, 11, _fieldLabels[1], _newProduct.Name, _focusState == FocusState.Content && _currentFieldIndex == 1 && !_isEditing);
        DrawNumericFormField(contentX, 13, _fieldLabels[2], _newProduct.Quantity, _focusState == FocusState.Content && _currentFieldIndex == 2 && !_isEditing);
        DrawFormField(contentX, 15, _fieldLabels[3], _newProduct.Category, _focusState == FocusState.Content && _currentFieldIndex == 3 && !_isEditing);
        DrawNumericFormField(contentX, 17, _fieldLabels[4], _newProduct.MinQuantity, _focusState == FocusState.Content && _currentFieldIndex == 4 && !_isEditing);
        DrawFormField(contentX, 19, _fieldLabels[5], _newProduct.Description, _focusState == FocusState.Content && _currentFieldIndex == 5 && !_isEditing);
        DrawNumericFormField(contentX, 21, _fieldLabels[6], _newProduct.Price, _focusState == FocusState.Content && _currentFieldIndex == 6 && !_isEditing, true);
        string buttons = "[Enter/Tab] Editar | [↑/↓] Navegar | [Space]+[G] Guardar | [ESC/←] Menú";
        Console.SetCursorPosition(contentX, 25);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(buttons);
        if (!string.IsNullOrEmpty(_validationError)) { Console.SetCursorPosition(contentX, 27); Console.ForegroundColor = ConsoleColor.Red; Console.Write(_validationError); }
        Console.ResetColor();
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
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
        if (_isEditing) return this;
        if (_awaitingG_ForSave)
        {
            if (key.Key == ConsoleKey.G) { _awaitingG_ForSave = false; return ValidateAndSave(); }
            _awaitingG_ForSave = false;
        }
        if (key.Key == ConsoleKey.Spacebar) { _awaitingG_ForSave = true; _validationError = ""; return this; }
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow or ConsoleKey.Escape: _focusState = FocusState.Navigation; break;
            case ConsoleKey.DownArrow: _currentFieldIndex = (_currentFieldIndex + 1) % _fieldLabels.Length; break;
            case ConsoleKey.UpArrow: _currentFieldIndex = (_currentFieldIndex - 1 + _fieldLabels.Length) % _fieldLabels.Length; break;
            case ConsoleKey.Enter or ConsoleKey.Tab: EnterEditMode(_currentFieldIndex); break;
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
            if (result == EditResult.TabbedForward) { _currentFieldIndex = (_currentFieldIndex + 1) % _fieldLabels.Length; continue; }
            if (result == EditResult.Confirmed) { _currentFieldIndex = (_currentFieldIndex + 1) % _fieldLabels.Length; }
            break;
        }
        _isEditing = false;
        Draw();
    }
    private (string, EditResult) EditField(int fieldIndex)
    {
        int textX = 45, textW = 26, numX = 48, numW = 13;
        return fieldIndex switch
        {
            0 => new InputHandler(textX, 9, textW, _newProduct.Sku, InputType.Text).ProcessInput(),
            1 => new InputHandler(textX, 11, textW, _newProduct.Name, InputType.Text).ProcessInput(),
            2 => new InputHandler(numX, 13, numW, _newProduct.Quantity.ToString(), InputType.Integer).ProcessInput(),
            3 => new InputHandler(textX, 15, textW, _newProduct.Category, InputType.Text).ProcessInput(),
            4 => new InputHandler(numX, 17, numW, _newProduct.MinQuantity.ToString(), InputType.Integer).ProcessInput(),
            5 => new InputHandler(textX, 19, textW, _newProduct.Description, InputType.Text).ProcessInput(),
            6 => new InputHandler(numX, 21, numW, _newProduct.Price.ToString(), InputType.Decimal).ProcessInput(),
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
        if (string.IsNullOrWhiteSpace(_newProduct.Sku)) errors.Add("SKU obligatorio.");
        if (string.IsNullOrWhiteSpace(_newProduct.Name)) errors.Add("Producto obligatorio.");
        if (_newProduct.Quantity <= 0) errors.Add("Cantidad > 0.");
        if (_newProduct.Price <= 0) errors.Add("Precio > 0.");
        if (errors.Count > 0) { _validationError = "Error: " + string.Join(" ", errors); return this; }
        _inventoryManager.AddProduct(_newProduct);
        return new ShowProductsView(_inventoryManager);
    }
}


