using System;

/// <summary>
/// Un componente de TUI que representa un botón accionable.
/// </summary>
public class Button : TuiComponent
{
    public string Text { get; set; }

    public Button(int x, int y, string text)
        : base(x, y, text.Length + 4, 3) // +4 para padding
    {
        Text = text;
    }

    /// <summary>
    /// Dibuja el botón, cambiando su apariencia si tiene el foco.
    /// </summary>
    public override void Draw(TuiRenderer renderer)
    {
        ConsoleColor fg = ConsoleColor.White;
        ConsoleColor bg = ConsoleColor.DarkGray;

        if (HasFocus)
        {
            bg = ConsoleColor.Gray;
            fg = ConsoleColor.Black;
        }

        renderer.Write(X, Y, new string(' ', Width), bg: bg);
        renderer.Write(X + 2, Y, Text, fg, bg);
    }
}

