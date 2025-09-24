using System;
using System.Globalization;

/// <summary>
/// Un componente de TUI para la entrada y modificación de valores numéricos.
/// </summary>
public class NumericField : TuiComponent
{
    public decimal Value { get; set; }
    public bool IsCurrency { get; }
    public ValidationState State { get; set; } = ValidationState.Pristine;

    public NumericField(int x, int y, int width, bool isCurrency = false)
        : base(x, y, width, 3)
    {
        IsCurrency = isCurrency;
    }

    public void Clear()
    {
        Value = 0;
        State = ValidationState.Pristine;
    }

    /// <summary>
    /// Dibuja el campo numérico con sus botones y valor.
    /// </summary>
    public override void Draw(TuiRenderer renderer)
    {
        ConsoleColor borderColor = State switch
        {
            ValidationState.Valid => ConsoleColor.Green,
            ValidationState.Invalid => ConsoleColor.Red,
            _ => HasFocus ? ConsoleColor.DarkYellow : ConsoleColor.Gray
        };

        renderer.Write(X, Y + 1, "[+]", borderColor);
        new Frame(X + 4, Y, Width - 8, 3).Draw(renderer);
        renderer.Write(X + Width - 3, Y + 1, "[-]", borderColor);

        string displayValue = IsCurrency
            ? Value.ToString("C", new CultureInfo("es-CR"))
            : Value.ToString();

        renderer.Write(X + 6, Y + 1, displayValue.PadLeft(Width - 12));

        if (HasFocus)
        {
            Console.SetCursorPosition(X + 6 + displayValue.Length, Y + 1);
        }
    }

    /// <summary>
    /// Maneja las teclas de incremento, decremento y entrada numérica.
    /// </summary>
    public override void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.OemPlus || key.Key == ConsoleKey.Add)
        {
            Value++;
        }
        else if (key.Key == ConsoleKey.OemMinus || key.Key == ConsoleKey.Subtract)
        {
            if (Value > 0) Value--;
        }
        else if (key.Key == ConsoleKey.Enter)
        {
            // Al presionar Enter, se activa un InputHandler para edición directa.
            var inputHandler = new InputHandler(X + 6, Y + 1, Width - 12, Value.ToString(), IsCurrency ? InputType.Decimal : InputType.Integer);
            var (newValue, result) = inputHandler.ProcessInput();
            if (result != EditResult.Canceled)
            {
                decimal.TryParse(newValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue);
                Value = parsedValue;
            }
        }
    }
}
