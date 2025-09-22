using System;
using System.Collections.Generic;

/// <summary>
/// Un componente de TUI para mostrar datos en formato de tabla.
/// </summary>
public class Table : TuiComponent
{
    private List<string> _headers = new List<string>();
    private List<string[]> _rows = new List<string[]>();
    public int SelectedIndex { get; set; } = -1;

    public Table(int x, int y, int width, int height) : base(x, y, width, height) { }

    public void SetHeaders(params string[] headers)
    {
        _headers = new List<string>(headers);
    }

    public void AddRow(params string[] rowData)
    {
        _rows.Add(rowData);
    }

    public void ClearRows()
    {
        _rows.Clear();
    }

    public override void Draw(TuiRenderer renderer)
    {
        // Dibuja el marco de la tabla
        new Frame(X, Y, Width, Height).Draw(renderer);

        // Dibuja las cabeceras
        if (_headers.Count > 0)
        {
            int colWidth = (Width - 2) / _headers.Count;
            for (int i = 0; i < _headers.Count; i++)
            {
                renderer.Write(X + 1 + (i * colWidth), Y + 1, _headers[i], ConsoleColor.Yellow);
            }
        }

        // Dibuja las filas
        for (int i = 0; i < _rows.Count && i < Height - 3; i++)
        {
            var row = _rows[i];
            int colWidth = (Width - 2) / row.Length;
            for (int j = 0; j < row.Length; j++)
            {
                var fg = ConsoleColor.White;
                var bg = ConsoleColor.Black;
                if (i == SelectedIndex && HasFocus)
                {
                    fg = ConsoleColor.Black;
                    bg = ConsoleColor.Gray;
                }
                renderer.Write(X + 1 + (j * colWidth), Y + 3 + i, row[j].PadRight(colWidth), fg, bg);
            }
        }
    }

    public override void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.UpArrow)
        {
            SelectedIndex = Math.Max(0, SelectedIndex - 1);
        }
        else if (key.Key == ConsoleKey.DownArrow)
        {
            SelectedIndex = Math.Min(_rows.Count - 1, SelectedIndex + 1);
        }
    }
}

