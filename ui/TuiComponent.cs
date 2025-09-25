using System;

/// <summary>
/// Clase base abstracta para todos los componentes de la Interfaz de Usuario de Texto (TUI).
/// Define las propiedades y métodos comunes que todo componente visual debe tener.
/// </summary>
public abstract class TuiComponent
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    // Se convierte a una propiedad completa para un control más explícito y predecible.
    private bool _hasFocus;

    /// <summary>
    /// Obtiene o establece si el componente tiene el foco de la interfaz de usuario.
    /// Marcado como 'virtual' para permitir que las clases hijas (como ProductForm) lo sobrescriban.
    /// </summary>
    public virtual bool HasFocus
    {
        get => _hasFocus;
        set
        {
            // Asigna el nuevo valor del foco. El cambio visual se maneja en el método Draw del componente
            // que hereda de esta clase, el cual debe verificar esta propiedad antes de dibujarse.
            _hasFocus = value;
        }
    }

    protected TuiComponent(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Dibuja el componente en el lienzo virtual proporcionado por el TuiRenderer.
    /// Este método debe ser implementado por todas las clases concretas.
    /// </summary>
    /// <param name="renderer">El motor de renderizado a utilizar.</param>
    public abstract void Draw(TuiRenderer renderer);

    /// <summary>
    /// Maneja la entrada del teclado. La implementación base no hace nada.
    /// Las clases hijas pueden sobrescribir este método para proporcionar interactividad.
    /// </summary>
    /// <param name="key">La información de la tecla presionada.</param>
    public virtual void HandleInput(ConsoleKeyInfo key) { /* No-op by default */ }
}


