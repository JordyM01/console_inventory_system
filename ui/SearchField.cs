using System;
using System.Text;

/// <summary>
/// Un componente de TUI especializado para la búsqueda.
/// Dibuja un borde naranja y gestiona la visibilidad del cursor cuando tiene el foco.
/// </summary>
public class SearchField : TuiComponent
{
    private readonly StringBuilder _textBuilder;
    public string Text => _textBuilder.ToString();

    public SearchField(int x, int y, int width) : base(x, y, width, 3)
    {
        _textBuilder = new StringBuilder();
    }

    /// <summary>
    /// Dibuja el campo de búsqueda con un borde naranja fijo.
    /// </summary>
    public override void Draw(TuiRenderer renderer)
    {
        ConsoleColor borderColor = ConsoleColor.DarkYellow; // Siempre naranja

        // Dibuja el borde con el color especificado
        renderer.Write(X, Y, "+", borderColor);
        renderer.Write(X + Width - 1, Y, "+", borderColor);
        renderer.Write(X, Y + Height - 1, "+", borderColor);
        renderer.Write(X + Width - 1, Y + Height - 1, "+", borderColor);
        renderer.Write(X + 1, Y, new string('-', Width - 2), borderColor);
        renderer.Write(X + 1, Y + Height - 1, new string('-', Width - 2), borderColor);
        for (int i = 1; i < Height - 1; i++)
        {
            renderer.Write(X, Y + i, "|", borderColor);
            renderer.Write(X + Width - 1, Y + i, "|", borderColor);
        }

        string textToDisplay = Text;
        int displayWidth = Width - 4;

        if (textToDisplay.Length > displayWidth)
        {
            textToDisplay = "..." + textToDisplay.Substring(Text.Length - displayWidth + 3);
        }

        renderer.Write(X + 2, Y + 1, textToDisplay.PadRight(displayWidth));

        // El cursor solo es visible y se posiciona si la vista indica que este componente tiene el foco.
        if (HasFocus)
        {
            Console.CursorVisible = true;
            Console.SetCursorPosition(X + 2 + textToDisplay.Length, Y + 1);
        }
    }

    /// <summary>
    /// Procesa una tecla presionada por el usuario.
    /// </summary>
    /// <returns>True si el texto cambió, de lo contrario False.</returns>
    public bool HandleKey(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace && _textBuilder.Length > 0)
        {
            _textBuilder.Length--;
            return true;
        }
        if (!char.IsControl(key.KeyChar))
        {
            _textBuilder.Append(key.KeyChar);
            return true;
        }
        return false;
    }
}

