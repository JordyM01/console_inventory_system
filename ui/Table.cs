using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Representa la definición de una columna en el componente Table,
/// incluyendo su título y ancho.
/// </summary>
public class ColumnDefinition
{
    /// <summary>
    /// Obtiene el texto que se mostrará en la cabecera de la columna.
    /// </summary>
    public string Header { get; }

    /// <summary>
    /// Obtiene el ancho en caracteres de la columna.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ColumnDefinition"/>.
    /// </summary>
    /// <param name="header">El título de la cabecera de la columna.</param>
    /// <param name="width">El ancho en caracteres de la columna.</param>
    public ColumnDefinition(string header, int width)
    {
        Header = header;
        Width = width;
    }
}

/// <summary>
/// Representa una fila de datos dentro del componente Table, incluyendo
/// los datos de sus celdas y sus estilos de color.
/// </summary>
public class TableRow
{
    /// <summary>
    /// Obtiene el arreglo de strings que representa los datos de cada celda en la fila.
    /// </summary>
    public string[] CellData { get; }

    /// <summary>
    /// Obtiene el color de texto por defecto para la fila.
    /// </summary>
    public ConsoleColor ForegroundColor { get; }

    /// <summary>
    /// Obtiene el color de fondo por defecto para la fila.
    /// </summary>
    public ConsoleColor BackgroundColor { get; }

    /// <summary>
    /// Obtiene el color de texto para la fila cuando está seleccionada.
    /// </summary>
    public ConsoleColor SelectedForegroundColor { get; }

    /// <summary>
    /// Obtiene el color de fondo para la fila cuando está seleccionada.
    /// </summary>
    public ConsoleColor SelectedBackgroundColor { get; }

    /// <summary>
    /// Obtiene o establece un objeto de datos personalizado asociado a la fila.
    /// Útil para almacenar el objeto de modelo completo (ej. un objeto 'Product')
    /// y recuperarlo fácilmente cuando la fila es seleccionada.
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="TableRow"/>.
    /// </summary>
    /// <param name="cellData">El arreglo de strings con los datos de las celdas.</param>
    /// <param name="fg">Color de texto por defecto.</param>
    /// <param name="bg">Color de fondo por defecto.</param>
    /// <param name="selectedFg">Color de texto cuando la fila está seleccionada.</param>
    /// <param name="selectedBg">Color de fondo cuando la fila está seleccionada.</param>
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
/// Un componente de TUI reutilizable para mostrar datos en un formato de tabla interactiva,
/// con soporte para selección, desplazamiento (scroll) y espaciado de columnas.
/// </summary>
public class Table : TuiComponent
{
    private List<ColumnDefinition> _columns = new List<ColumnDefinition>();
    /// <summary>
    /// Obtiene una lista de solo lectura de las definiciones de columna de la tabla.
    /// </summary>
    public IReadOnlyList<ColumnDefinition> Columns => _columns.AsReadOnly();

    private List<TableRow> _rows = new List<TableRow>();

    /// <summary>
    /// Obtiene o establece el índice basado en cero de la fila actualmente seleccionada.
    /// Un valor de -1 indica que no hay ninguna fila seleccionada.
    /// </summary>
    public int SelectedIndex { get; set; } = -1;

    private int _scrollTop = 0;
    /// <summary>
    /// Obtiene el índice de la primera fila visible en la tabla, usado para la lógica de desplazamiento.
    /// </summary>
    public int ScrollTop => _scrollTop;


    /// <summary>
    /// Inicializa una nueva instancia del componente <see cref="Table"/>.
    /// </summary>
    /// <param name="x">La coordenada X de la esquina superior izquierda de la tabla.</param>
    /// <param name="y">La coordenada Y de la esquina superior izquierda de la tabla.</param>
    /// <param name="width">El ancho total de la tabla.</param>
    /// <param name="height">El alto total de la tabla.</param>
    public Table(int x, int y, int width, int height) : base(x, y, width, height) { }

    // --- MÉTODOS PÚBLICOS (API del Componente) ---

    /// <summary>
    /// Establece las columnas que se mostrarán en la tabla.
    /// </summary>
    /// <param name="columns">Un arreglo de objetos ColumnDefinition.</param>
    public void SetColumns(params ColumnDefinition[] columns)
    {
        _columns = columns.ToList();
    }

    /// <summary>
    /// Añade una nueva fila de datos al final de la tabla.
    /// </summary>
    /// <param name="row">El objeto TableRow que se va a añadir.</param>
    public void AddRow(TableRow row)
    {
        _rows.Add(row);
    }

    /// <summary>
    /// Obtiene el objeto TableRow completo de la fila actualmente seleccionada.
    /// </summary>
    /// <returns>El objeto TableRow seleccionado, o null si no hay ninguna selección.</returns>
    public TableRow? GetSelectedRow()
    {
        if (SelectedIndex >= 0 && SelectedIndex < _rows.Count)
        {
            return _rows[SelectedIndex];
        }
        return null;
    }

    /// <summary>
    /// Elimina todas las filas de la tabla y resetea la selección y el desplazamiento.
    /// </summary>
    public void ClearRows()
    {
        _rows.Clear();
        SelectedIndex = -1;
        _scrollTop = 0;
    }

    // --- LÓGICA INTERNA ---

    /// <summary>
    /// Dibuja el componente de tabla completo, incluyendo el marco, las cabeceras y las filas visibles.
    /// </summary>
    /// <param name="renderer">El motor de renderizado para dibujar en la consola.</param>
    public override void Draw(TuiRenderer renderer)
    {
        new Frame(X, Y, Width, Height).Draw(renderer);

        // 1. Dibujar Cabeceras con espaciado entre columnas
        int currentX = X + 2; // Margen izquierdo
        foreach (var column in _columns)
        {
            renderer.Write(currentX, Y + 1, column.Header.PadRight(column.Width), ConsoleColor.Yellow);
            // Se añade 1 espacio para separar las columnas ---
            currentX += column.Width + 1;
        }

        // 2. Dibujar Filas sin espaciado vertical
        // Se calcula el espacio disponible para filas contiguas ---
        int availableRows = Height - 3; // Espacio para borde sup/inf y cabecera
        for (int i = 0; i < availableRows; i++)
        {
            int rowIndex = _scrollTop + i;
            // Las filas se dibujan una después de la otra ---
            int currentLineY = Y + 2 + i;

            // Limpia la línea antes de dibujar para evitar artefactos visuales
            renderer.Write(X + 1, currentLineY, new string(' ', Width - 2));

            if (rowIndex < _rows.Count)
            {
                var row = _rows[rowIndex];
                bool isSelected = (rowIndex == SelectedIndex && HasFocus);

                var fg = isSelected ? row.SelectedForegroundColor : row.ForegroundColor;
                var bg = isSelected ? row.SelectedBackgroundColor : row.BackgroundColor;

                currentX = X + 2; // Resetea la posición X para la nueva fila
                for (int j = 0; j < _columns.Count; j++)
                {
                    string cellText = (j < row.CellData.Length) ? row.CellData[j] : "";
                    int colWidth = _columns[j].Width;

                    if (cellText.Length > colWidth) cellText = cellText.Substring(0, colWidth);

                    renderer.Write(currentX, currentLineY, cellText.PadRight(colWidth), fg, bg);
                    // Se añade 1 espacio para separar las columnas ---
                    currentX += colWidth + 1;
                }
            }
        }
    }

    /// <summary>
    /// Maneja la entrada del teclado para la navegación vertical dentro de la tabla (flechas, inicio, fin, etc.).
    /// </summary>
    /// <param name="key">La información de la tecla presionada por el usuario.</param>
    public override void HandleInput(ConsoleKeyInfo key)
    {
        // Se ajusta el cálculo de filas visibles para la lógica de scroll ---
        int availableRows = Height - 3;
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                if (SelectedIndex > 0) SelectedIndex--;
                if (SelectedIndex < _scrollTop) _scrollTop = SelectedIndex;
                break;

            case ConsoleKey.DownArrow:
                if (SelectedIndex < _rows.Count - 1) SelectedIndex++;
                if (SelectedIndex >= _scrollTop + availableRows) _scrollTop++;
                break;

            case ConsoleKey.Home:
                SelectedIndex = 0;
                _scrollTop = 0;
                break;

            case ConsoleKey.End:
                SelectedIndex = _rows.Count - 1;
                _scrollTop = Math.Max(0, SelectedIndex - availableRows + 1);
                break;

            case ConsoleKey.PageUp:
                SelectedIndex = Math.Max(0, SelectedIndex - availableRows);
                _scrollTop = Math.Max(0, _scrollTop - availableRows);
                break;

            case ConsoleKey.PageDown:
                SelectedIndex = Math.Min(_rows.Count - 1, SelectedIndex + availableRows);
                _scrollTop = Math.Min(Math.Max(0, _rows.Count - availableRows), _scrollTop + availableRows);
                break;
        }
    }
}

