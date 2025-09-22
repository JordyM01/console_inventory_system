using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Vista del menú principal, compuesta por componentes de TUI.
/// </summary>
public class MainMenuView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly SideBar _sideBar;
    private readonly Frame _frame;
    private readonly Label _title;

    public MainMenuView(InventoryManager manager)
    {
        _inventoryManager = manager;

        _frame = new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);
        _title = new Label((Console.WindowWidth - "Sistema de gestion de inventario en consola".Length) / 2, 5, "Sistema de gestion de inventario en consola", ConsoleColor.Green);

        _sideBar = new SideBar(2, 8, 14, NavigationHelper.MenuItems.ToList(), "Inicio")
        {
            HasFocus = true,
            SelectedIndex = 1
        };
    }

    public void Draw(TuiRenderer renderer)
    {
        Console.CursorVisible = false;

        _frame.Draw(renderer);
        _title.Draw(renderer);
        _sideBar.Draw(renderer);
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        _sideBar.HandleInput(key);

        if (key.Key == ConsoleKey.Enter)
        {
            return NavigationHelper.GetViewByIndex(_sideBar.SelectedIndex, _inventoryManager);
        }

        return this;
    }
}


