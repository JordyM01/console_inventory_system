using System.Text;

/// <summary>
/// Representa un campo de texto interactivo y reutilizable en la consola.
/// </summary>
public class TextField
{
    private readonly int _x, _y, _width;
    private readonly StringBuilder _textBuilder;
    public string Text => _textBuilder.ToString();

    public TextField(int x, int y, int width, string initialText = "")
    {
        _x = x;
        _y = y;
        _width = width;
        _textBuilder = new StringBuilder(initialText);
    }

    /// <summary>
    /// Dibuja el campo de texto y su contenido, incluyendo el efecto de barrido.
    /// </summary>
    public void Draw()
    {
        UiComponents.DrawBox(_x, _y, _width, 3, ConsoleColor.DarkYellow);

        string textToDisplay = Text;
        int displayWidth = _width - 4; // Espacio para bordes y padding

        if (textToDisplay.Length > displayWidth)
        {
            textToDisplay = "..." + textToDisplay.Substring(textToDisplay.Length - displayWidth + 3);
        }

        Console.SetCursorPosition(_x + 2, _y + 1);
        Console.Write(new string(' ', displayWidth)); // Limpia
        Console.SetCursorPosition(_x + 2, _y + 1);
        Console.Write(textToDisplay); // Escribe

        // Posiciona el cursor al final del texto para la entrada del usuario
        Console.SetCursorPosition(_x + 2 + textToDisplay.Length, _y + 1);
    }

    /// <summary>
    /// Procesa una tecla. Devuelve true si el texto cambió.
    /// </summary>
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



