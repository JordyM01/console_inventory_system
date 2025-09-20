using static UiComponents;

public class AboutView : IView
{
    private readonly InventoryManager _inventoryManager;
    private FocusState _focusState = FocusState.Content;
    private int _navigationIndex = 5;

    public AboutView(InventoryManager manager) => _inventoryManager = manager;

    public void Draw()
    {
        DrawLayout("Acerca del software", _focusState);
        Console.CursorVisible = false;
        int contentX = 27, contentY = 3;
        Console.SetCursorPosition(contentX, contentY);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("/ Acerca del software");
        int boxY = contentY + 4;
        DrawBox(contentX, boxY, Console.WindowWidth - contentX - 2, 12, ConsoleColor.Green);

        string[] aboutLines = {
            "Sistema de Gestión de Inventario", "Versión 1.0.0", "",
            "Desarrollado por: JordyM", "Proyecto de Aplicación de Consola en .NET",
            "", "Copyright (c) 2025",
        };

        for (int i = 0; i < aboutLines.Length; i++)
        {
            Console.SetCursorPosition(contentX + 2, boxY + 2 + i);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(aboutLines[i]);
        }
        Console.ResetColor();
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
        if (_focusState == FocusState.Navigation)
        {
            if (key.Key is ConsoleKey.Enter or ConsoleKey.RightArrow)
            {
                var nextView = NavigationHelper.GetViewByIndex(_navigationIndex, _inventoryManager);
                if (nextView is AboutView) { _focusState = FocusState.Content; return this; }
                return nextView;
            }
            // CORRECCIÓN (CS1501): Se elimina el tercer argumento 'manager'.
            NavigationHelper.HandleMenuNavigation(key, ref _navigationIndex);
            return this;
        }

        if (key.Key is ConsoleKey.Escape or ConsoleKey.LeftArrow)
        {
            _focusState = FocusState.Navigation;
        }
        return this;
    }
}


