using System;

public class DeleteProductView : IView
{
    private readonly InventoryManager _inventoryManager;
    public DeleteProductView(InventoryManager manager) => _inventoryManager = manager;

    public void Draw(TuiRenderer renderer)
    {
        new Label(30, 10, "Vista de Eliminar no implementada aún.").Draw(renderer);
    }

    public IView? HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape) return new MainMenuView(_inventoryManager);
        return this;
    }
}
