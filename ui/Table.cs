using System;
using System.Collections.Generic;

/// <summary>
/// Un componente de TUI para mostrar datos en formato de tabla.
/// </summary>
public class Table : TuiComponent
{
    private List<Label> _headers = new List<Label>();
    private List<string[]> _rows = new List<string[]>();
    public int SelectedIndex { get; set; } = -1;
    private int _scrollTop = 0; // Indice del primer producto visible en la tabla

    public Table(int x, int y, int width, int height) : base(x, y, width, height) { }

    public void SetHeaders(List<Label> headers)
    {
        _headers = headers;
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
            for (int i = 0; i < _headers.Count; i++)
            {
                _headers[i].Draw(renderer);
            }
        }

        int availableRows = (Height - 3) / 2; // Espacio para el borde y espaciado de linea

        // Dibuja las filas
        for (int i = 0; i < availableRows; i++)
        {
            int productIndex = _scrollTop + i;
            int currentLineY = Y + 3 + (i * 2); // Deja una linea en blanco entre cada fila

            // Limpia la linea anter de dibujar
            renderer.Write(X + 1, currentLineY, new string(' ', Width - 2));

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

