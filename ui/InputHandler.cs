using System.Text;

public enum InputType { Text, Integer, Decimal }
public enum EditResult { Confirmed, Canceled, TabbedForward }

public class InputHandler
{
    private readonly int _x, _y, _width;
    private readonly InputType _type;
    private readonly string _originalValue;

    public InputHandler(int x, int y, int width, string initialValue, InputType type)
    {
        _x = x;
        _y = y;
        _width = width;
        _originalValue = initialValue;
        _type = type;
    }

    public (string, EditResult) ProcessInput()
    {
        var textBuilder = new StringBuilder(_originalValue);
        Console.CursorVisible = true;

        while (true)
        {
            DrawInputText(textBuilder.ToString());
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter) return (textBuilder.ToString(), EditResult.Confirmed);
            if (key.Key == ConsoleKey.Escape) return (_originalValue, EditResult.Canceled);
            if (key.Key == ConsoleKey.Tab) return (textBuilder.ToString(), EditResult.TabbedForward);

            switch (_type)
            {
                case InputType.Text: HandleTextInput(key, textBuilder); break;
                case InputType.Integer: HandleNumericInput(key, textBuilder, allowDecimal: false); break;
                case InputType.Decimal: HandleNumericInput(key, textBuilder, allowDecimal: true); break;
            }
        }
    }

    private void HandleTextInput(ConsoleKeyInfo key, StringBuilder textBuilder)
    {
        if (key.Key == ConsoleKey.Backspace && textBuilder.Length > 0) textBuilder.Length--;
        else if (!char.IsControl(key.KeyChar)) textBuilder.Append(key.KeyChar);
    }
    private void HandleNumericInput(ConsoleKeyInfo key, StringBuilder textBuilder, bool allowDecimal)
    {
        string currentText = textBuilder.ToString();
        if (key.Key is ConsoleKey.OemPlus or ConsoleKey.Add)
        {
            if (decimal.TryParse(currentText, out decimal val)) textBuilder.Clear().Append(val + 1);
            else textBuilder.Clear().Append("1");
        }
        else if (key.Key is ConsoleKey.OemMinus or ConsoleKey.Subtract)
        {
            if (decimal.TryParse(currentText, out decimal val) && val > 0) textBuilder.Clear().Append(val - 1);
        }
        else if (key.Key == ConsoleKey.Backspace && textBuilder.Length > 0) textBuilder.Length--;
        else if (char.IsDigit(key.KeyChar)) textBuilder.Append(key.KeyChar);
        else if (allowDecimal && (key.KeyChar == '.' || key.KeyChar == ',') && !currentText.Contains('.') && !currentText.Contains(','))
        {
            textBuilder.Append('.');
        }
    }

    private void DrawInputText(string text)
    {
        Console.SetCursorPosition(_x, _y);
        // --- CAMBIO DE COLOR A ALTO CONTRASTE ---
        Console.BackgroundColor = ConsoleColor.Cyan;    // Fondo cyan para la edición
        Console.ForegroundColor = ConsoleColor.Black;   // Texto negro para máxima legibilidad

        string textToDisplay = text;
        if (text.Length > _width)
        {
            textToDisplay = "..." + text.Substring(text.Length - _width + 3);
        }

        Console.Write(textToDisplay.PadRight(_width));
        Console.SetCursorPosition(_x + textToDisplay.Length, _y);
        Console.ResetColor();
    }
}


