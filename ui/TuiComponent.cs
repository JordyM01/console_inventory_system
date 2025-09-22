using System;

/// <summary>
/// Clase base abstracta para todos los componentes de la TUI.
/// Define las propiedades y métodos comunes como posición, tamaño y foco.
/// </summary>
public abstract class TuiComponent
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool HasFocus { get; set; }

    protected TuiComponent(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Dibuja el componente en el buffer de renderizado.
    /// </summary>
    public abstract void Draw(TuiRenderer renderer);

    /// <summary>
    /// Maneja la entrada del teclado cuando el componente tiene el foco.
    /// </summary>
    public virtual void HandleInput(ConsoleKeyInfo key) { }
}

