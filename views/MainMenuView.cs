using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista del menú principal
/// Se compone de componentes de TUI y gestiona una navegación en cuadrícula.
/// </summary>
public class MainMenuView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly List<TuiComponent> _components = new List<TuiComponent>();
    private readonly List<MenuItem> _menuItems = new List<MenuItem>();
    private int _focusIndex = 0; // El índice del MenuItem que tiene el foco

    public MainMenuView(InventoryManager manager)
    {
        _inventoryManager = manager;

        // --- Diseño Dinámico ---
        // Las posiciones y tamaños se calculan usando Console.WindowWidth y Console.WindowHeight.

        int framePadding = 3;
        int titleFrameHeight = 5;
        int titleY = framePadding + 2;
        string title = "Sistema de gestion de inventario en consola";

        // Componentes estáticos (se añaden a la lista general para ser dibujados)
        _components.Add(new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1));
        _components.Add(new Frame(framePadding, framePadding, Console.WindowWidth - (framePadding * 2), titleFrameHeight));
        _components.Add(new Label(
            (Console.WindowWidth - title.Length) / 2,
            titleY,
            title,
            ConsoleColor.Green
        ));

        // Se calculan las posiciones de los items del menú de forma proporcional.
        int col1X = Console.WindowWidth / 4;
        int col2X = Console.WindowWidth / 2 + 5;
        int menuStartY = titleFrameHeight + framePadding + 5;
        int menuSpacingY = 4;

        // Componentes de menú (se añaden a su propia lista para gestionar el foco)
        // Las posiciones X se calculan como fracciones del ancho de la ventana.
        _menuItems.Add(new MenuItem(col1X, menuStartY, "Agregar producto   [ A ]", ConsoleKey.A));
        _menuItems.Add(new MenuItem(col1X, menuStartY + menuSpacingY, "Mostrar productos  [ S ]", ConsoleKey.S));
        _menuItems.Add(new MenuItem(col1X, menuStartY + menuSpacingY * 2, "Actualizar producto[ U ]", ConsoleKey.U));
        _menuItems.Add(new MenuItem(col2X, menuStartY, "Eliminar producto  [ D ]", ConsoleKey.D));
        _menuItems.Add(new MenuItem(col2X, menuStartY + menuSpacingY, "Acerca del software[ I ]", ConsoleKey.I));
        _menuItems.Add(new MenuItem(col2X, menuStartY + menuSpacingY * 2, "Salir              [ESC]", ConsoleKey.Escape));

        _components.AddRange(_menuItems);
        UpdateFocus();
    }

    /// <summary>
    /// Actualiza qué elemento del menú tiene el foco visual.
    /// </summary>
    private void UpdateFocus()
    {
        for (int i = 0; i < _menuItems.Count; i++)
        {
            _menuItems[i].HasFocus = (i == _focusIndex);
        }
    }

    public void Draw(TuiRenderer renderer)
    {
        Console.CursorVisible = false;
        foreach (var component in _components)
        {
            component.Draw(renderer);
        }
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        // Manejo de atajos de teclado
        var hotkeyItem = _menuItems.FirstOrDefault(item => item.HotKey == key.Key);
        if (hotkeyItem != null)
        {
            // Si se presiona un atajo, se navega directamente
            int itemIndex = _menuItems.IndexOf(hotkeyItem);
            return SelectItem(itemIndex);
        }

        // Manejo de navegación con flechas en la cuadrícula 2x3
        // Lógica de navegación en cuadrícula mejorada con envoltura (wrapping)
        switch (key.Key)
        {
            case ConsoleKey.DownArrow:
                if (_focusIndex >= 3) // Columna derecha
                    _focusIndex = 3 + ((_focusIndex - 3 + 1) % 3);
                else // Columna izquierda
                    _focusIndex = (_focusIndex + 1) % 3;
                break;
            case ConsoleKey.UpArrow:
                if (_focusIndex >= 3) // Columna derecha
                    _focusIndex = 3 + ((_focusIndex - 3 - 1 + 3) % 3);
                else // Columna izquierda
                    _focusIndex = (_focusIndex - 1 + 3) % 3;
                break;
            case ConsoleKey.RightArrow:
                if (_focusIndex < 3) _focusIndex += 3;
                break;
            case ConsoleKey.LeftArrow:
                if (_focusIndex >= 3) _focusIndex -= 3;
                break;
            case ConsoleKey.Enter:
                return SelectItem(_focusIndex);
        }

        UpdateFocus();
        return this; // Permanece en esta vista
    }

    /// <summary>
    /// Navega a la vista correspondiente al índice del item seleccionado.
    /// </summary>
    private IView? SelectItem(int index)
    {
        // Mapea el índice del menú principal al índice del NavigationHelper
        // (ej. "Agregar producto" es el 0 aquí, pero el 1 en el menú lateral)
        return index switch
        {
            0 => NavigationHelper.GetViewByIndex(1, _inventoryManager), // Agregar
            1 => NavigationHelper.GetViewByIndex(2, _inventoryManager), // Mostrar
            2 => NavigationHelper.GetViewByIndex(3, _inventoryManager), // Actualizar
            3 => NavigationHelper.GetViewByIndex(4, _inventoryManager), // Eliminar
            4 => NavigationHelper.GetViewByIndex(5, _inventoryManager), // Acerca de
            5 => NavigationHelper.GetViewByIndex(6, _inventoryManager), // Salir
            _ => this
        };
    }
}


