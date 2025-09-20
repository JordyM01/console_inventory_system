/// <summary>
/// Clase estática con métodos de ayuda para dibujar elementos comunes de la interfaz de usuario.
/// </summary>
public static class UiComponents
{
    /// <summary>
    /// Dibuja el layout base con el panel de navegación izquierdo.
    /// El menú solo se resalta si el foco está en la navegación.
    /// </summary>
    public static void DrawLayout(string currentViewName, FocusState focus)
    {
        int leftPanelWidth = 25;
        DrawBox(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);

        // Dibuja la línea vertical divisora
        for (int y = 1; y < Console.WindowHeight - 2; y++)
        {
            Console.SetCursorPosition(leftPanelWidth, y);
            Console.Write("|");
        }

        // Dibuja los elementos del menú
        for (int i = 0; i < NavigationHelper.MenuItems.Length; i++)
        {
            Console.SetCursorPosition(4, 5 + i * 2);
            string activeItemName = currentViewName == "MainMenuView" ? "Inicio" : currentViewName;

            // Resalta el elemento activo solo si el foco está en el panel de navegación
            if (focus == FocusState.Navigation && NavigationHelper.MenuItems[i] == activeItemName)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($" {NavigationHelper.MenuItems[i]} ");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(NavigationHelper.MenuItems[i]);
            }
        }
    }

    /// <summary>
    /// Dibuja una caja simple con bordes de caracteres.
    /// </summary>
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

    /// <summary>
    /// Dibuja un campo de texto con etiqueta y borde.
    /// </summary>
    public static void DrawFormField(int x, int y, string label, string value, bool isFocused)
    {
        Console.SetCursorPosition(x, y);
        Console.Write($"{label.PadRight(15)}");
        // El borde se resalta si el campo tiene el foco
        DrawBox(x + 16, y - 1, 30, 3, isFocused ? ConsoleColor.DarkYellow : ConsoleColor.Gray);

        // Limpia el área de texto antes de escribir
        Console.SetCursorPosition(x + 18, y);
        Console.Write(new string(' ', 26));

        // Escribe el valor
        Console.SetCursorPosition(x + 18, y);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(value);
        Console.ResetColor();
    }

    /// <summary>
    /// Dibuja un campo numérico con botones de incremento/decremento.
    /// </summary>
    public static void DrawNumericFormField(int x, int y, string label, object value, bool isFocused, bool isCurrency = false)
    {
        Console.SetCursorPosition(x, y);
        Console.Write($"{label.PadRight(15)}");
        Console.SetCursorPosition(x + 16, y); Console.Write("[+]");
        DrawBox(x + 20, y - 1, 15, 3, isFocused ? ConsoleColor.DarkYellow : ConsoleColor.Gray);
        Console.SetCursorPosition(x + 36, y); Console.Write("[-]");

        string displayValue = isCurrency ? $"{value:C}" : value.ToString();
        Console.SetCursorPosition(x + 21, y);
        Console.Write(new string(' ', 13)); // Limpia el área
        Console.SetCursorPosition(x + 21, y);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(displayValue.PadLeft(13));
        Console.ResetColor();
    }
}


