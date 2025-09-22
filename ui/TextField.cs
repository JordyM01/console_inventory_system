using System;
using System.Text;

public enum ValidationState { Pristine, Valid, Invalid }

/// <summary>
/// Un componente de TUI para la entrada de texto interactiva.
/// </summary>
public class TextField : TuiComponent
{
    private StringBuilder _textBuilder;
    public string Text { get => _textBuilder.ToString(); set => _textBuilder = new StringBuilder(value); }
    public ValidationState State { get; set; } = ValidationState.Pristine;

    public TextField(int x, int y, int width) : base(x, y, width, 3)
    {
        _textBuilder = new StringBuilder();
    }

    public override void Draw(TuiRenderer renderer)
    {
        ConsoleColor borderColor = State switch
        {
            ValidationState.Valid => ConsoleColor.Green,
            ValidationState.Invalid => ConsoleColor.Red,
            _ => HasFocus ? ConsoleColor.DarkYellow : ConsoleColor.Gray
        };

        // Dibuja el borde
        renderer.Write(X, Y, "+" + new string('-', Width - 2) + "+", borderColor);
        renderer.Write(X, Y + 1, "|", borderColor);
        renderer.Write(X + Width - 1, Y + 1, "|", borderColor);
        renderer.Write(X, Y + 2, "+" + new string('-', Width - 2) + "+", borderColor);

        // Dibuja el texto con efecto de barrido
        string textToDisplay = Text;
        int displayWidth = Width - 4;
        if (textToDisplay.Length > displayWidth)
        {
            textToDisplay = "..." + textToDisplay.Substring(Text.Length - displayWidth + 3);
        }

        renderer.Write(X + 2, Y + 1, textToDisplay.PadRight(displayWidth));

        if (HasFocus)
        {
            Console.CursorVisible = true;
            Console.SetCursorPosition(X + 2 + textToDisplay.Length, Y + 1);
        }
    }

    public override void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace && _textBuilder.Length > 0)
        {
            _textBuilder.Length--;
        }
        else if (!char.IsControl(key.KeyChar))
        {
            _textBuilder.Append(key.KeyChar);
        }
    }
}


