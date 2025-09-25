using System;

/// <summary>
/// Un componente de TUI que dibuja un diálogo modal para confirmaciones.
/// Se encarga de dibujar un fondo sólido para ocultar el contenido subyacente.
/// </summary>
public class ConfirmationDialog : TuiComponent
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string Options { get; set; }

    public ConfirmationDialog(string title, string message, string options)
        : base(0, 0, 0, 0) // La posición y tamaño se calculan dinámicamente
    {
        Title = title;
        Message = message;
        Options = options;
    }

    public override void Draw(TuiRenderer renderer)
    {
        // Calcula las dimensiones y la posición para centrar el diálogo
        Width = Math.Max(Message.Length, Title.Length) + 6;
        Height = 7;
        X = (Console.WindowWidth - Width) / 2;
        Y = (Console.WindowHeight - Height) / 2;

        // Dibuja un fondo sólido para ocultar lo que hay detrás
        for (int i = 0; i < Height; i++)
        {
            renderer.Write(X, Y + i, new string(' ', Width), bg: ConsoleColor.Black);
        }

        // Dibuja el marco y el texto del diálogo
        new Frame(X, Y, Width, Height).Draw(renderer);
        renderer.Write(X + (Width - Title.Length) / 2, Y + 1, Title, ConsoleColor.White, ConsoleColor.Red);
        renderer.Write(X + (Width - Message.Length) / 2, Y + 3, Message, ConsoleColor.White);
        renderer.Write(X + (Width - Options.Length) / 2, Y + 5, Options, ConsoleColor.Yellow);
    }
}
