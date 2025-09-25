using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

/// <summary>
/// Componente de UI reutilizable para mostrar y editar los campos de un producto.
/// Encapsula la lógica de navegación, validación y edición de un formulario.
/// </summary>
public class ProductForm : TuiComponent
{
    private readonly List<(Label Label, TuiComponent Field)> _formItems = new();
    private readonly List<TuiComponent> _focusableComponents = new();

    private int _focusIndex = 0;
    private int _scrollTop = 0;
    private string _statusMessage = "";
    private Product? _product;

    // Campos del formulario
    private readonly TextField _idField, _skuField, _productField, _categoryField, _descField;
    private readonly NumericField _qtyField, _minQtyField, _priceField;
    private readonly Button _cancelButton, _saveButton;

    // Eventos para comunicar la finalización a la vista contenedora
    public Action<Product>? OnSave { get; set; }
    public Action? OnCancel { get; set; }

    /// <summary>
    /// Obtiene o establece el estado de foco del formulario.
    /// Cuando el formulario pierde el foco, se asegura de que todos sus componentes hijos también lo pierdan.
    /// </summary>
    public override bool HasFocus
    {
        get => base.HasFocus;
        set
        {
            base.HasFocus = value;
            if (value)
            {
                UpdateFocus();
            }
            else
            {
                foreach (var component in _focusableComponents)
                {
                    component.HasFocus = false;
                }
            }
        }
    }

    public ProductForm(int x, int y, int width, int height) : base(x, y, width, height)
    {
        _idField = new TextField(0, 0, 25) { IsEditable = false };
        _skuField = new TextField(0, 0, 40);
        _productField = new TextField(0, 0, 40);
        _qtyField = new NumericField(0, 0, 20);
        _categoryField = new TextField(0, 0, 40);
        _minQtyField = new NumericField(0, 0, 20);
        _descField = new TextField(0, 0, 40);
        _priceField = new NumericField(0, 0, 20, isCurrency: true);
        _cancelButton = new Button(0, 0, "Cancelar / ESC");
        _saveButton = new Button(0, 0, "Guardar / Enter");

        _formItems.Add((new Label(0, 0, "Id"), _idField));
        _formItems.Add((new Label(0, 0, "SKU"), _skuField));
        _formItems.Add((new Label(0, 0, "Producto"), _productField));
        _formItems.Add((new Label(0, 0, "Cantidad"), _qtyField));
        _formItems.Add((new Label(0, 0, "Categoría"), _categoryField));
        _formItems.Add((new Label(0, 0, "Cant Mínima"), _minQtyField));
        _formItems.Add((new Label(0, 0, "Descripción"), _descField));
        _formItems.Add((new Label(0, 0, "Precio"), _priceField));
        _formItems.Add((new Label(0, 0, ""), _cancelButton));
        _formItems.Add((new Label(0, 0, ""), _saveButton));

        _focusableComponents.AddRange(_formItems.Where(item => item.Field != _idField).Select(item => item.Field));

        Clear();
    }

    public void LoadProduct(Product product)
    {
        _product = product;
        _idField.Text = product.Id;
        _skuField.Text = product.Sku;
        _productField.Text = product.Name;
        _qtyField.Value = product.Quantity;
        _categoryField.Text = product.Category;
        _minQtyField.Value = product.MinQuantity;
        _descField.Text = product.Description;
        _priceField.Value = product.Price;
    }

    public void Clear()
    {
        _skuField.Clear(); _productField.Clear(); _qtyField.Clear(); _categoryField.Clear();
        _minQtyField.Clear(); _descField.Clear(); _priceField.Clear();
        _idField.Text = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        _statusMessage = "";
        _product = null;
        _focusIndex = 0;
        _scrollTop = 0;
        UpdateFocus();
    }

    private void UpdateFocus()
    {
        for (int i = 0; i < _focusableComponents.Count; i++)
        {
            _focusableComponents[i].HasFocus = (HasFocus && i == _focusIndex);
        }
    }

    public override void Draw(TuiRenderer renderer)
    {
        new Frame(X, Y, Width, Height).Draw(renderer);
        string title = _product == null ? "Nuevo Producto" : $"Detalles de {_product.Name}";
        new Label(X + 2, Y, $"-- {title} --").Draw(renderer);

        // --- Dibuja el campo ID (siempre fijo en la parte superior) ---
        var (idLabel, idField) = _formItems[0];
        int idFieldY = Y + 2;
        idLabel.X = X + 5;
        idLabel.Y = idFieldY + 1;
        idField.X = X + 20;
        idField.Y = idFieldY;
        if (idFieldY > Y && idFieldY < Y + Height - 4)
        {
            idLabel.Draw(renderer);
            idField.Draw(renderer);
        }

        // --- Dibuja los campos de entrada que sí se desplazan ---
        int scrollableItemIndex = 0;
        for (int i = 1; i < _formItems.Count; i++)
        {
            var (label, field) = _formItems[i];
            if (field is Button) continue;

            int displayIndex = scrollableItemIndex - _scrollTop;
            int currentY = Y + 5 + (displayIndex * 3);

            label.X = X + 5;
            label.Y = currentY + 1;
            field.X = X + 20;
            field.Y = currentY;

            if (currentY > Y + 3 && currentY < Y + Height - 4)
            {
                label.Draw(renderer);
                field.Draw(renderer);
            }
            scrollableItemIndex++;
        }

        // --- Dibuja los botones (siempre fijos en la parte inferior) ---
        int buttonY = Y + Height - 3;
        _cancelButton.X = X + 20;
        _cancelButton.Y = buttonY;
        _saveButton.X = X + _cancelButton.Width + 23;
        _saveButton.Y = buttonY;
        _cancelButton.Draw(renderer);
        _saveButton.Draw(renderer);

        if (!string.IsNullOrEmpty(_statusMessage))
            renderer.Write(X + 2, Y + Height - 2, _statusMessage.PadRight(Width - 4), ConsoleColor.Black, ConsoleColor.Red);

        var focused = _focusableComponents[_focusIndex];
        Console.CursorVisible = HasFocus && (focused is TextField tf && tf.IsEditable || focused is NumericField);
    }

    public override void HandleInput(ConsoleKeyInfo key)
    {
        if (!HasFocus) return;

        var focusedComponent = _focusableComponents[_focusIndex];

        if (key.Key == ConsoleKey.Enter && focusedComponent is not Button)
        {
            EnterEditMode(_focusIndex);
            return;
        }
        else
        {
            focusedComponent.HandleInput(key);
        }

        if (key.Key is ConsoleKey.DownArrow or ConsoleKey.Tab)
        {
            _focusIndex = (_focusIndex + 1) % _focusableComponents.Count;
        }
        else if (key.Key is ConsoleKey.UpArrow)
        {
            _focusIndex = (_focusIndex - 1 + _focusableComponents.Count) % _focusableComponents.Count;
        }
        else if (key.Key == ConsoleKey.Escape)
        {
            OnCancel?.Invoke();
            return;
        }

        int maxVisibleFields = (Height - 8) / 3;
        if (_focusIndex >= _scrollTop + maxVisibleFields)
        {
            _scrollTop = _focusIndex - maxVisibleFields + 1;
        }
        if (_focusIndex < _scrollTop)
        {
            _scrollTop = _focusIndex;
        }

        UpdateFocus();

        if (focusedComponent == _saveButton && key.Key == ConsoleKey.Enter) ValidateAndSave();
        if (focusedComponent == _cancelButton && key.Key == ConsoleKey.Enter) OnCancel?.Invoke();
    }

    private void EnterEditMode(int fieldIndex)
    {
        var focusedComponent = _focusableComponents[fieldIndex];
        var (newValue, result) = ("", EditResult.Canceled);

        if (focusedComponent is TextField tf && tf.IsEditable)
        {
            var inputHandler = new InputHandler(tf.X + 2, tf.Y + 1, tf.Width - 4, tf.Text, InputType.Text);
            (newValue, result) = inputHandler.ProcessInput();
            if (result != EditResult.Canceled) tf.Text = newValue;
        }
        else if (focusedComponent is NumericField nf)
        {
            var inputHandler = new InputHandler(nf.X + 6, nf.Y + 1, nf.Width - 12, nf.Value.ToString(CultureInfo.InvariantCulture), nf.IsCurrency ? InputType.Decimal : InputType.Integer);
            (newValue, result) = inputHandler.ProcessInput();
            if (result != EditResult.Canceled)
            {
                decimal.TryParse(newValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue);
                nf.Value = parsedValue;
            }
        }
    }

    private void ValidateAndSave()
    {
        _statusMessage = "";
        _skuField.State = _productField.State = _qtyField.State = _priceField.State = ValidationState.Pristine;

        bool isValid = true;
        if (string.IsNullOrWhiteSpace(_idField.Text))
        {
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(_skuField.Text)) { _skuField.State = ValidationState.Invalid; isValid = false; }
        if (string.IsNullOrWhiteSpace(_productField.Text)) { _productField.State = ValidationState.Invalid; isValid = false; }
        if (_qtyField.Value < 0) { _qtyField.State = ValidationState.Invalid; isValid = false; }
        if (_priceField.Value <= 0) { _priceField.State = ValidationState.Invalid; isValid = false; }

        if (!isValid)
        {
            _statusMessage = "Error: Por favor, corrija los campos marcados en rojo.";
            return;
        }

        var productToSave = new Product(
            Id: _idField.Text, Sku: _skuField.Text, Name: _productField.Text, Quantity: (int)_qtyField.Value,
            Category: _categoryField.Text, MinQuantity: (int)_minQtyField.Value,
            Description: _descField.Text, Price: _priceField.Value
        );
        OnSave?.Invoke(productToSave);
    }
}
