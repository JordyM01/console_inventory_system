using System;
using System.Text;

public enum ValidationState { Pristine, Valid, Invalid }

/// <summary>
/// Un componente de TUI para la entrada de texto interactiva.
/// </summary>
public class TextField : TuiComponent
{
    private StringBuilder _textBuilder;
    public string Text { get => _textBuilder.ToString(); set => _textBuilder = new StringBuilder(value); }
    public ValidationState State { get; set; } = ValidationState.Pristine;

    // Permite que un campo de texto sea de solo lectura (ej. para el campo Id).
    public bool IsEditable { get; set; } = true;

    public TextField(int x, int y, int width) : base(x, y, width, 3)
    {
        _textBuilder = new StringBuilder();
    }

    /// <summary>
    /// Limpia el contenido y el estado de validación del campo.
    /// </summary>
    public void Clear()
    {
        _textBuilder.Clear();
        State = ValidationState.Pristine;
    }

    /// <summary>
    /// Dibuja el componente, incluyendo su borde y fondo, basado en el estado de foco y validación.
    /// </summary>
    public override void Draw(TuiRenderer renderer)
    {
        // 1. Determina los colores correctos para el estado actual.
        ConsoleColor borderColor;
        ConsoleColor textColor = IsEditable ? ConsoleColor.White : ConsoleColor.DarkGray;
        // El color de fondo cambia si el campo tiene foco.
        ConsoleColor backgroundColor = HasFocus ? ConsoleColor.DarkCyan : ConsoleColor.Black;

        // El estado de validación tiene prioridad sobre el color de foco para el borde.
        switch (State)
        {
            case ValidationState.Valid:
                borderColor = ConsoleColor.Green;
                break;
            case ValidationState.Invalid:
                borderColor = ConsoleColor.Red;
                break;
            default: // Pristine
                borderColor = HasFocus ? ConsoleColor.DarkYellow : ConsoleColor.Gray;
                break;
        }

        // 2. Dibuja el borde del componente.
        renderer.Write(X, Y, "+" + new string('-', Width - 2) + "+", borderColor);
        renderer.Write(X, Y + 1, "|", borderColor);
        renderer.Write(X + Width - 1, Y + 1, "|", borderColor);
        renderer.Write(X, Y + 2, "+" + new string('-', Width - 2) + "+", borderColor);

        // 3. Prepara el texto a mostrar con efecto de barrido si es muy largo.
        string textToDisplay = Text;
        int displayWidth = Width - 4;
        if (textToDisplay.Length > displayWidth)
        {
            textToDisplay = "..." + textToDisplay.Substring(textToDisplay.Length - displayWidth + 3);
        }

        // 4. Limpia el área de texto y dibuja el contenido con el color de fondo correcto.
        // Esto es clave para eliminar el resaltado cuando el componente pierde el foco.
        renderer.Write(X + 2, Y + 1, textToDisplay.PadRight(displayWidth), textColor, backgroundColor);

        // 5. Gestiona la visibilidad y posición del cursor.
        // El cursor solo debe ser visible si este componente específico tiene el foco y es editable.
        if (HasFocus && IsEditable)
        {
            Console.CursorVisible = true;
            Console.SetCursorPosition(X + 2 + textToDisplay.Length, Y + 1);
        }
    }
}

