using System;
using System.Collections.Generic;

/// <summary>
/// Vista para agregar un nuevo producto, compuesta por componentes de TUI.
/// </summary>
public class AddProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly List<TuiComponent> _components;
    private readonly SideBar _sideBar;
    private readonly List<TextField> _textFields;
    private int _focusIndex = 1;

    public AddProductView(InventoryManager manager)
    {
        _inventoryManager = manager;
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Agregar producto");

        _textFields = new List<TextField>
        {
            new TextField(45, 9, 30) { Text = Guid.NewGuid().ToString("N").Substring(0,8).ToUpper() },
            new TextField(45, 12, 30),
            new TextField(45, 15, 30),
        };

        _components = new List<TuiComponent> { _sideBar };
        _components.AddRange(_textFields);
        UpdateFocus();
    }

    private void UpdateFocus()
    {
        for (int i = 0; i < _components.Count; i++)
        {
            _components[i].HasFocus = (i == _focusIndex);
        }
    }

    public void Draw(TuiRenderer renderer)
    {
        foreach (var component in _components)
        {
            component.Draw(renderer);
        }
        renderer.Write(30, 10, "ID:");
        renderer.Write(30, 13, "SKU:");
        renderer.Write(30, 16, "Producto:");
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.LeftArrow) _focusIndex = 0;
        if (key.Key == ConsoleKey.RightArrow) _focusIndex = 1;
        if (key.Key == ConsoleKey.UpArrow && _focusIndex > 1) _focusIndex--;
        if (key.Key == ConsoleKey.DownArrow && _focusIndex > 0 && _focusIndex < _components.Count - 1) _focusIndex++;

        UpdateFocus();
        _components[_focusIndex].HandleInput(key);

        if (_focusIndex == 0 && key.Key == ConsoleKey.Enter)
        {
            return NavigationHelper.GetViewByIndex(_sideBar.SelectedIndex, _inventoryManager);
        }

        return this;
    }
}


