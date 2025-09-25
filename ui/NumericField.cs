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
    /// Dibuja el campo numérico con sus botones y valor, controlando el color de fondo.
    /// </summary>
    public override void Draw(TuiRenderer renderer)
    {
        // 1. Determina los colores correctos para el estado actual.
        ConsoleColor borderColor;
        ConsoleColor textColor = ConsoleColor.White;
        // El color de fondo cambia si el campo tiene foco.
        ConsoleColor backgroundColor = HasFocus ? ConsoleColor.DarkCyan : ConsoleColor.Black;

        switch (State)
        {
            case ValidationState.Valid:
                borderColor = ConsoleColor.Green;
                break;
            case ValidationState.Invalid:
                borderColor = ConsoleColor.Red;
                break;
            default: // Pristine
                borderColor = HasFocus ? ConsoleColor.DarkYellow : ConsoleColor.Gray;
                break;
        }

        // 2. Dibuja los elementos estáticos del componente.
        renderer.Write(X, Y + 1, "[+]", borderColor);
        new Frame(X + 4, Y, Width - 8, 3).Draw(renderer); // El marco para el valor.
        renderer.Write(X + Width - 3, Y + 1, "[-]", borderColor);

        // 3. Formatea el valor numérico a mostrar.
        string displayValue = IsCurrency
            ? Value.ToString("C", new CultureInfo("es-CR")) // Formato de moneda para Costa Rica
            : Value.ToString();

        int displayWidth = Width - 12; // Ancho del área de texto dentro del marco.

        // Se asegura de que el texto no exceda el ancho y lo alinea a la derecha.
        if (displayValue.Length > displayWidth)
        {
            displayValue = displayValue.Substring(0, displayWidth);
        }

        // 4. Dibuja el valor con el color de fondo correcto.
        // Este es el paso crucial que soluciona el problema del resaltado persistente.
        renderer.Write(X + 6, Y + 1, displayValue.PadLeft(displayWidth), textColor, backgroundColor);
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
            // Al presionar Enter, se activa un InputHandler para edición directa del valor.
            var inputHandler = new InputHandler(X + 6, Y + 1, Width - 12, Value.ToString(CultureInfo.InvariantCulture), IsCurrency ? InputType.Decimal : InputType.Integer);
            var (newValue, result) = inputHandler.ProcessInput();
            if (result != EditResult.Canceled)
            {
                decimal.TryParse(newValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue);
                Value = parsedValue;
            }
        }
    }
}

