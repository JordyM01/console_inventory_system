using System;
using System.Globalization;

/// <summary>
/// Un componente de TUI para la entrada y modificación de valores numéricos.
/// </summary>
public class NumericField : TuiComponent
{
    public decimal Value { get; set; }
    private bool _isCurrency;

    public NumericField(int x, int y, int width, bool isCurrency = false)
        : base(x, y, width, 3)
    {
        _isCurrency = isCurrency;
    }

    /// <summary>
    /// Dibuja el campo numérico con sus botones y valor.
    /// </summary>
    public override void Draw(TuiRenderer renderer)
    {
        ConsoleColor borderColor = HasFocus ? ConsoleColor.DarkYellow : ConsoleColor.Gray;

        // Dibuja los componentes del campo
        renderer.Write(X, Y + 1, "[+]", borderColor);
        new Frame(X + 4, Y, Width - 8, 3).Draw(renderer);
        renderer.Write(X + Width - 3, Y + 1, "[-]", borderColor);

        // Formatea y dibuja el valor
        string displayValue = _isCurrency ? Value.ToString("C", new CultureInfo("es-CR")) : Value.ToString();
        renderer.Write(X + 6, Y + 1, displayValue.PadLeft(Width - 12));
    }

    /// <summary>
    /// Maneja las teclas de incremento, decremento y entrada numérica.
    /// </summary>
    public override void HandleInput(ConsoleKeyInfo key)
    {
        // La lógica de edición de texto se manejaría con una clase como InputHandler
        // aquí, por simplicidad, solo se implementa el incremento/decremento.
        if (key.Key == ConsoleKey.OemPlus || key.Key == ConsoleKey.Add)
        {
            Value++;
        }
        else if (key.Key == ConsoleKey.OemMinus || key.Key == ConsoleKey.Subtract)
        {
            if (Value > 0) Value--;
        }
    }
}
