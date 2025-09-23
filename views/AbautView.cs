using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista "Acerca de", compuesta por componentes de TUI.
/// </summary>
public class AboutView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly List<TuiComponent> _components = new List<TuiComponent>();
    private readonly SideBar _sideBar;
    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 5; // Índice de "Acerca del software"

    public AboutView(InventoryManager manager, int lastNavIndex = 5)
    {
        _inventoryManager = manager;
        _navigationIndex = lastNavIndex;

        // --- Composición de la Vista ---
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Acerca del software");
        _sideBar.SelectedIndex = _navigationIndex;

        _components.Add(new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1));
        _components.Add(_sideBar);
        _components.Add(new Label(27, 4, "/ Acerca del software", ConsoleColor.White));

        // Contenedor para la información
        _components.Add(new Frame(27, 8, Console.WindowWidth - 29, 15));

        // Información
        _components.Add(new Label(30, 10, "Sistema de Gestion de Inventario", ConsoleColor.Cyan));
        _components.Add(new Label(30, 11, "Version 0.1.1", ConsoleColor.Cyan));
        _components.Add(new Label(30, 13, "Desarrollado por:", ConsoleColor.White));
        _components.Add(new Label(30, 14, "Jordy Javier Mairena Montoya", ConsoleColor.Cyan));
        _components.Add(new Label(30, 15, "Carnet: 2025-3361U", ConsoleColor.Cyan));
        _components.Add(new Label(30, 17, "Carrera: Ingenieria en sistemas de Informacion", ConsoleColor.White));
        _components.Add(new Label(30, 18, "Universidad Nacional de Ingenieria - UNI", ConsoleColor.Cyan));

        UpdateFocus();
    }

    private void UpdateFocus()
    {
        _sideBar.HasFocus = _focusState == FocusState.Navigation;
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
        if (_focusState == FocusState.Navigation)
        {
            _sideBar.HandleInput(key);

            if (key.Key is ConsoleKey.Enter or ConsoleKey.RightArrow)
            {
                var nextView = NavigationHelper.GetViewByIndex(_sideBar.SelectedIndex, _inventoryManager);
                // Si la vista seleccionada es esta misma, solo cambia el foco
                if (nextView is AboutView)
                {
                    _focusState = FocusState.Content;
                    UpdateFocus();
                    return this;
                }
                return nextView;
            }
        }
        else // FocusState.Content
        {
            // El contenido de esta vista no es interactivo, solo cambia el foco
            if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow)
            {
                _focusState = FocusState.Navigation;
                UpdateFocus();
            }
        }

        return this;
    }
}
