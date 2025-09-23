using System;
using System.Text;

public struct ScreenChar
{
    public char Char;
    public ConsoleColor Foreground;
    public ConsoleColor Background;
}

/// <summary>
/// Motor de renderizado optimizado para la TUI.
/// </summary>
public class TuiRenderer
{
    private ScreenChar[,] _currentBuffer;
    private ScreenChar[,] _nextBuffer;
    private readonly int _width;
    private readonly int _height;

    public TuiRenderer()
    {
        Console.OutputEncoding = Encoding.UTF8;
        _width = Console.WindowWidth;
        _height = Console.WindowHeight;
        _currentBuffer = new ScreenChar[_width, _height];
        _nextBuffer = new ScreenChar[_width, _height];
        ClearBuffer(_currentBuffer);
        ClearBuffer(_nextBuffer);
    }

    public void ClearBuffer(ScreenChar[,] buffer)
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                buffer[x, y] = new ScreenChar { Char = ' ', Foreground = ConsoleColor.Gray, Background = ConsoleColor.Black };
            }
        }
    }

    public void ClearBuffer() => ClearBuffer(_nextBuffer);

    public void Write(int x, int y, string text, ConsoleColor fg = ConsoleColor.Gray, ConsoleColor bg = ConsoleColor.Black)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (x + i >= 0 && x + i < _width && y >= 0 && y < _height)
            {
                _nextBuffer[x + i, y] = new ScreenChar { Char = text[i], Foreground = fg, Background = bg };
            }
        }
    }

    public void Render()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_currentBuffer[x, y].Char != _nextBuffer[x, y].Char ||
                    _currentBuffer[x, y].Foreground != _nextBuffer[x, y].Foreground ||
                    _currentBuffer[x, y].Background != _nextBuffer[x, y].Background)
                {
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = _nextBuffer[x, y].Foreground;
                    Console.BackgroundColor = _nextBuffer[x, y].Background;
                    Console.Write(_nextBuffer[x, y].Char);
                    _currentBuffer[x, y] = _nextBuffer[x, y];
                }
            }
        }
        Console.ResetColor();
    }
}
