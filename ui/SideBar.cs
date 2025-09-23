using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Componente que gestiona un menú de navegación lateral.
/// </summary>
public class SideBar : TuiComponent
{
    private readonly List<string> _items;
    public int SelectedIndex { get; set; } = 0;
    public string ActiveViewName { get; set; }

    public SideBar(int x, int y, int height, List<string> items, string activeViewName)
        : base(x, y, 25, height)
    {
        _items = items;
        ActiveViewName = activeViewName;
    }

    public override void Draw(TuiRenderer renderer)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            string prefix = (ActiveViewName == _items[i] && !HasFocus) ? "> " : "  ";
            string textToDraw = prefix + _items[i];
            ConsoleColor fg = ConsoleColor.Cyan;
            ConsoleColor bg = ConsoleColor.Black;

            if (HasFocus && i == SelectedIndex)
            {
                fg = ConsoleColor.Black;
                bg = ConsoleColor.Cyan;
                textToDraw = " " + textToDraw;
            }
            // Se escribe primero una línea de fondo para limpiar cualquier artefacto visual
            // de la renderización anterior, y luego se escribe el texto encima.
            renderer.Write(X, Y + i * 2, new string(' ', Width - X), bg: ConsoleColor.Black);
            renderer.Write(X, Y + i * 2, textToDraw.PadRight(Width - X), fg, bg);

        }
    }

    public override void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.UpArrow)
        {
            SelectedIndex = (SelectedIndex > 0) ? SelectedIndex - 1 : _items.Count - 1;
        }
        else if (key.Key == ConsoleKey.DownArrow)
        {
            SelectedIndex = (SelectedIndex < _items.Count - 1) ? SelectedIndex + 1 : 0;
        }
    }
}

