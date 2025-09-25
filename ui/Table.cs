using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Define la estructura y el ancho de una columna en el componente Table.
/// </summary>
public class ColumnDefinition
{
    public string Header { get; }
    public int Width { get; }

    public ColumnDefinition(string header, int width)
    {
        Header = header;
        Width = width;
    }
}

/// <summary>
/// Representa una fila de datos dentro del componente Table, incluyendo su estilo y un objeto de datos asociado.
/// </summary>
public class TableRow
{
    public string[] CellData { get; }
    public ConsoleColor ForegroundColor { get; }
    public ConsoleColor BackgroundColor { get; }
    public ConsoleColor SelectedForegroundColor { get; }
    public ConsoleColor SelectedBackgroundColor { get; }

    /// <summary>
    /// Permite almacenar el objeto original (ej. un objeto Product) asociado a esta fila.
    /// Esencial para recuperar el dato completo al seleccionar una fila.
    /// </summary>
    public object? Tag { get; set; }

    public TableRow(string[] cellData, ConsoleColor fg = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black,
                    ConsoleColor selectedFg = ConsoleColor.Black, ConsoleColor selectedBg = ConsoleColor.Gray)
    {
        CellData = cellData;
        ForegroundColor = fg;
        BackgroundColor = bg;
        SelectedForegroundColor = selectedFg;
        SelectedBackgroundColor = selectedBg;
    }
}

/// <summary>
/// Un componente de TUI reutilizable para mostrar datos en una tabla interactiva con scroll y selección.
/// </summary>
public class Table : TuiComponent
{
    private List<ColumnDefinition> _columns = new List<ColumnDefinition>();
    private List<TableRow> _rows = new List<TableRow>();

    public int SelectedIndex { get; set; } = -1;
    private int _scrollTop = 0; // Índice de la primera fila visible

    public int ScrollTop => _scrollTop;
    public IReadOnlyList<ColumnDefinition> Columns => _columns.AsReadOnly();

    public Table(int x, int y, int width, int height) : base(x, y, width, height) { }

    /// <summary>
    /// Establece las columnas que tendrá la tabla.
    /// </summary>
    /// <param name="columns">Una o más definiciones de columna.</param>
    public void SetColumns(params ColumnDefinition[] columns)
    {
        _columns = columns.ToList();
    }

    /// <summary>
    /// Añade una fila de datos a la tabla.
    /// </summary>
    /// <param name="row">El objeto TableRow que contiene los datos y el estilo.</param>
    public void AddRow(TableRow row)
    {
        _rows.Add(row);
    }

    /// <summary>
    /// Obtiene la fila actualmente seleccionada.
    /// </summary>
    /// <returns>El objeto TableRow seleccionado, o null si no hay selección.</returns>
    public TableRow? GetSelectedRow()
    {
        if (SelectedIndex >= 0 && SelectedIndex < _rows.Count)
        {
            return _rows[SelectedIndex];
        }
        return null;
    }

    /// <summary>
    /// Limpia todas las filas de la tabla y resetea la selección.
    /// </summary>
    public void ClearRows()
    {
        _rows.Clear();
        SelectedIndex = -1;
        _scrollTop = 0;
    }

    // --- LÓGICA INTERNA DE RENDERIZADO Y MANEJO DE INPUT ---

    public override void Draw(TuiRenderer renderer)
    {
        // Dibuja el marco exterior de la tabla
        new Frame(X, Y, Width, Height).Draw(renderer);

        // 1. Dibuja las cabeceras basadas en las definiciones de columna
        int currentX = X + 2; // Margen izquierdo
        foreach (var column in _columns)
        {
            renderer.Write(currentX, Y + 1, column.Header.PadRight(column.Width), ConsoleColor.Yellow);
            currentX += column.Width;
        }

        // 2. Dibuja las filas visibles
        int availableRows = Height - 3; // Espacio disponible restando bordes y cabecera
        for (int i = 0; i < availableRows; i++)
        {
            int rowIndex = _scrollTop + i;
            int currentLineY = Y + 2 + i;

            // Limpia la línea completa antes de dibujar para evitar artefactos visuales
            renderer.Write(X + 1, currentLineY, new string(' ', Width - 2));

            if (rowIndex < _rows.Count)
            {
                var row = _rows[rowIndex];
                bool isSelected = (rowIndex == SelectedIndex && HasFocus);

                // Determina el color basado en si la fila está seleccionada y tiene foco
                var fg = isSelected ? row.SelectedForegroundColor : row.ForegroundColor;
                var bg = isSelected ? row.SelectedBackgroundColor : row.BackgroundColor;

                currentX = X + 2; // Resetea la posición X para la nueva fila
                for (int j = 0; j < _columns.Count; j++)
                {
                    string cellText = (j < row.CellData.Length) ? row.CellData[j] : "";
                    int colWidth = _columns[j].Width;

                    // Trunca el texto si excede el ancho de la columna
                    if (cellText.Length > colWidth) cellText = cellText.Substring(0, colWidth);

                    renderer.Write(currentX, currentLineY, cellText.PadRight(colWidth), fg, bg);
                    currentX += colWidth;
                }
            }
        }
    }

    /// <summary>
    /// Maneja la navegación del usuario dentro de la tabla (flechas, inicio, fin, etc.).
    /// </summary>
    public override void HandleInput(ConsoleKeyInfo key)
    {
        int availableRows = Height - 3;
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                if (SelectedIndex > 0) SelectedIndex--;
                // Si la selección sube por encima del área visible, desplaza el scroll hacia arriba
                if (SelectedIndex < _scrollTop) _scrollTop = SelectedIndex;
                break;

            case ConsoleKey.DownArrow:
                if (SelectedIndex < _rows.Count - 1) SelectedIndex++;
                // Si la selección baja por debajo del área visible, desplaza el scroll hacia abajo
                if (SelectedIndex >= _scrollTop + availableRows) _scrollTop++;
                break;

            case ConsoleKey.Home: // Va al primer elemento
                SelectedIndex = 0;
                _scrollTop = 0;
                break;

            case ConsoleKey.End: // Va al último elemento
                SelectedIndex = _rows.Count - 1;
                _scrollTop = Math.Max(0, SelectedIndex - availableRows + 1);
                break;

            case ConsoleKey.PageUp: // Retrocede una "página" de items
                SelectedIndex = Math.Max(0, SelectedIndex - availableRows);
                _scrollTop = Math.Max(0, _scrollTop - availableRows);
                break;

            case ConsoleKey.PageDown: // Avanza una "página" de items
                SelectedIndex = Math.Min(_rows.Count - 1, SelectedIndex + availableRows);
                _scrollTop = Math.Min(Math.Max(0, _rows.Count - availableRows), _scrollTop + availableRows);
                break;
        }
    }
}
