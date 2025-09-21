public static class UiComponents
{
    /// <summary>
    /// Dibuja un panel con los detalles de un producto en formato de lista vertical.
    /// </summary>
    public static void DrawProductDetailsPanel(Product product, int x, int y, int width, int height)
    {
        // Dibuja un fondo sólido para el panel para ocultar el contenido subyacente.
        Console.BackgroundColor = ConsoleColor.Black;
        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write(new string(' ', width));
        }

        DrawBox(x, y, width, height, ConsoleColor.Green);

        string title = product.Name;
        Console.SetCursorPosition(x + (width - title.Length) / 2, y + 1);

        // --- CORRECCIÓN ---
        // Se establece el color de fondo a Negro para que sea consistente con el resto del panel.
        // El color del texto ahora es Verde para que resalte.
        // Esto soluciona tanto la inconsistencia visual como el artefacto al cerrar.
        Console.ForegroundColor = ConsoleColor.Green;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(title);
        Console.ResetColor();

        // Dibuja los campos y valores como una lista vertical
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(x + 3, y + 3);
        Console.Write($"ID:          {product.Id}");
        Console.SetCursorPosition(x + 3, y + 4);
        Console.Write($"SKU:         {product.Sku}");
        Console.SetCursorPosition(x + 3, y + 5);
        Console.Write($"Categoría:   {product.Category}");
        Console.SetCursorPosition(x + 3, y + 6);
        Console.Write($"Cantidad:    {product.Quantity}");
        Console.SetCursorPosition(x + 3, y + 7);
        Console.Write($"Cant. Mín:   {product.MinQuantity}");
        Console.SetCursorPosition(x + 3, y + 8);
        Console.Write($"Precio:      {product.Price:C}");

        string description = $"Descripción: {product.Description}";
        Console.SetCursorPosition(x + 3, y + 10);
        Console.Write(description.PadRight(width - 6).Substring(0, width - 6));

        string closeMessage = "[Presione cualquier tecla para cerrar]";
        Console.SetCursorPosition(x + (width - closeMessage.Length) / 2, y + height - 2);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(closeMessage);
        Console.ResetColor();
    }

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

            string prefix = (focus == FocusState.Content && menuItem == currentViewName) ? "> " : "  ";
            string textToDraw = prefix + menuItem;

            Console.Write(new string(' ', leftPanelWidth - 2));
            Console.SetCursorPosition(2, currentLineY);

            if (focus == FocusState.Navigation && i == navigationIndex)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                textToDraw = " " + textToDraw;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            Console.Write(textToDraw);
            Console.ResetColor();
        }
    }
    public static void DrawConfirmationDialog(string title, string message, string options)
    {
        int width = Math.Max(message.Length, title.Length) + 6;
        int height = 7;
        int startX = (Console.WindowWidth - width) / 2;
        int startY = (Console.WindowHeight - height) / 2;
        Console.BackgroundColor = ConsoleColor.Black;
        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(startX, startY + i);
            Console.Write(new string(' ', width));
        }
        DrawBox(startX, startY, width, height, ConsoleColor.Red);
        Console.SetCursorPosition(startX + (width - title.Length) / 2, startY + 1);
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Red;
        Console.Write(title);
        Console.ResetColor();
        Console.SetCursorPosition(startX + (width - message.Length) / 2, startY + 3);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(message);
        Console.SetCursorPosition(startX + (width - options.Length) / 2, startY + 5);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(options);
        Console.ResetColor();
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


