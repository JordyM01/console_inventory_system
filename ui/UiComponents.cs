public static class UiComponents
{
    public static void DrawLayout(string currentViewName, int navigationIndex, FocusState focus)
    {
        int leftPanelWidth = 25;
        DrawBox(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);

        for (int y = 1; y < Console.WindowHeight - 2; y++)
        {
            Console.SetCursorPosition(leftPanelWidth, y);
            Console.Write("|");
        }

        for (int i = 0; i < NavigationHelper.MenuItems.Length; i++)
        {
            string menuItem = NavigationHelper.MenuItems[i];
            int currentLineY = 5 + i * 2;
            Console.SetCursorPosition(2, currentLineY);

            // Determina el prefijo. El marcador ">" solo se muestra si el foco está en el contenido.
            string prefix = (focus == FocusState.Content && menuItem == currentViewName) ? "> " : "  ";
            string textToDraw = prefix + menuItem;

            // Establece los colores y el texto final a dibujar
            if (focus == FocusState.Navigation && i == navigationIndex)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                textToDraw = " " + textToDraw; // Añade padding para el efecto de resaltado
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }

            // Dibuja el texto, rellenando el resto de la línea con espacios.
            // Esto limpia y escribe en una sola operación, evitando artefactos visuales.
            Console.Write(textToDraw.PadRight(leftPanelWidth - 3));
            Console.ResetColor();
        }
    }

    public static void DrawBox(int x, int y, int width, int height, ConsoleColor color = ConsoleColor.Gray)
    {
        if (width < 2 || height < 2) return;
        Console.ForegroundColor = color;
        string hLine = "+" + new string('-', width - 2) + "+";
        Console.SetCursorPosition(x, y); Console.Write(hLine);
        for (int i = 1; i < height - 1; i++)
        {
            Console.SetCursorPosition(x, y + i); Console.Write("|");
            Console.SetCursorPosition(x + width - 1, y + i); Console.Write("|");
        }
        Console.SetCursorPosition(x, y + height - 1); Console.Write(hLine);
        Console.ResetColor();
    }

    public static void DrawFormField(int x, int y, string label, string value, bool isFocused)
    {
        Console.SetCursorPosition(x, y);
        Console.Write($"{label.PadRight(15)}");
        DrawBox(x + 16, y - 1, 30, 3, isFocused ? ConsoleColor.DarkYellow : ConsoleColor.Gray);

        Console.SetCursorPosition(x + 18, y);
        Console.Write(new string(' ', 26));

        Console.SetCursorPosition(x + 18, y);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(value);
        Console.ResetColor();
    }

    public static void DrawNumericFormField(int x, int y, string label, object value, bool isFocused, bool isCurrency = false)
    {
        Console.SetCursorPosition(x, y);
        Console.Write($"{label.PadRight(15)}");
        Console.SetCursorPosition(x + 16, y); Console.Write("[+]");
        DrawBox(x + 20, y - 1, 15, 3, isFocused ? ConsoleColor.DarkYellow : ConsoleColor.Gray);
        Console.SetCursorPosition(x + 36, y); Console.Write("[-]");

        string displayValue = isCurrency ? $"{value:C}" : value.ToString();
        Console.SetCursorPosition(x + 21, y);
        Console.Write(new string(' ', 13));
        Console.SetCursorPosition(x + 21, y);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(displayValue.PadLeft(13));
        Console.ResetColor();
    }
}


