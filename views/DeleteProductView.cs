using System;
using System.Linq;

public class DeleteProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    private readonly Frame _frame;
    private readonly SideBar _sideBar;

    public DeleteProductView(InventoryManager manager, int lastNavIndex = 4)
    {
        _inventoryManager = manager;
        _frame = new Frame(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);
        _sideBar = new SideBar(2, 5, 14, NavigationHelper.MenuItems.ToList(), "Eliminar producto");
        _sideBar.SelectedIndex = lastNavIndex;
        _sideBar.HasFocus = true;
    }

    public void Draw(TuiRenderer renderer)
    {
        _frame.Draw(renderer);
        _sideBar.Draw(renderer);
        new Label(30, 10, "Vista de Eliminar no implementada aún.").Draw(renderer);
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


