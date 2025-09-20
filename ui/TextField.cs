using System.Text;

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

    public void Draw()
    {
        UiComponents.DrawBox(_x, _y, _width, 3, ConsoleColor.DarkYellow);

        string textToDisplay = Text;
        int displayWidth = _width - 4;

        if (textToDisplay.Length > displayWidth)
        {
            textToDisplay = "..." + textToDisplay.Substring(textToDisplay.Length - displayWidth + 3);
        }

        Console.SetCursorPosition(_x + 2, _y + 1);
        Console.Write(new string(' ', displayWidth));
        Console.SetCursorPosition(_x + 2, _y + 1);
        Console.Write(textToDisplay);

        // --- CORRECCIÓN ---
        // Se elimina la línea Console.CursorVisible = true; de aquí.
        // La vista que usa este componente ahora es responsable de la visibilidad del cursor.
        Console.SetCursorPosition(_x + 2 + textToDisplay.Length, _y + 1);
    }

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


