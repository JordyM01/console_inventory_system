using System;
using System.Linq;

public class UpdateProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly Frame _frame;
    private readonly SideBar _sideBar;

    public UpdateProductView(InventoryManager manager, int lastNavIndex = 3)
    {
        _inventoryManager = manager;
        _frame = new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Actualizar producto");
        _sideBar.SelectedIndex = lastNavIndex;
        _sideBar.HasFocus = true; // El foco inicia en la navegación
    }

    public void Draw(TuiRenderer renderer)
    {
        _frame.Draw(renderer);
        _sideBar.Draw(renderer);
        new Label(30, 10, "Vista de Actualizar no implementada aún.").Draw(renderer);
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

