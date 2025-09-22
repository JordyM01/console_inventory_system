/// <summary>
/// Un componente de TUI simple que dibuja un borde o marco rectangular.
/// </summary>
public class Frame : TuiComponent
{
    public Frame(int x, int y, int width, int height)
        : base(x, y, width, height)
    {
    }

    /// <summary>
    /// Dibuja el marco en el buffer de renderizado.
    /// </summary>
    public override void Draw(TuiRenderer renderer)
    {
        if (Width < 2 || Height < 2) return;

        // Dibuja las esquinas
        renderer.Write(X, Y, "+");
        renderer.Write(X + Width - 1, Y, "+");
        renderer.Write(X, Y + Height - 1, "+");
        renderer.Write(X + Width - 1, Y + Height - 1, "+");

        // Dibuja las líneas horizontales
        for (int i = 1; i < Width - 1; i++)
        {
            renderer.Write(X + i, Y, "-");
            renderer.Write(X + i, Y + Height - 1, "-");
        }

        // Dibuja las líneas verticales
        for (int i = 1; i < Height - 1; i++)
        {
            renderer.Write(X, Y + i, "|");
            renderer.Write(X + Width - 1, Y + i, "|");
        }
    }
}

