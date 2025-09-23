using System;

/// <summary>
/// Representa un único elemento de menú seleccionable en la TUI.
/// Este componente gestiona su propio estado de dibujado (normal o resaltado).
/// </summary>
public class MenuItem : TuiComponent
{
    public string Text { get; set; }

    // El atajo de teclado asociado a este item.
    public ConsoleKey HotKey { get; }

    public MenuItem(int x, int y, string text, ConsoleKey hotKey)
        : base(x, y, text.Length + 4, 1) // +4 para el prefijo y padding
    {
        Text = text;
        HotKey = hotKey;
    }

    /// <summary>
    /// Dibuja el elemento del menú en el buffer del renderizador.
    /// Cambia su apariencia si tiene el foco.
    /// </summary>
    public override void Draw(TuiRenderer renderer)
    {
        if (HasFocus)
        {
            // Dibuja el item resaltado
            renderer.Write(X, Y, ">> ", ConsoleColor.Cyan);
            renderer.Write(X + 3, Y, Text, ConsoleColor.Black, ConsoleColor.Cyan);
        }
        else
        {
            // Dibuja el item en su estado normal
            renderer.Write(X + 3, Y, Text, ConsoleColor.Cyan);
        }
    }
}

