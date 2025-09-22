using System;

/// <summary>
/// Un componente de TUI para mostrar texto estático.
/// </summary>
public class Label : TuiComponent
{
    public string Text { get; set; }
    public ConsoleColor Color { get; set; }

    public Label(int x, int y, string text, ConsoleColor color = ConsoleColor.Gray)
        : base(x, y, text.Length, 1)
    {
        Text = text;
        Color = color;
    }

    public override void Draw(TuiRenderer renderer)
    {
        renderer.Write(X, Y, Text, Color);
    }
}

