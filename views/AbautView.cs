using static UiComponents;

// --- Vista de "Acerca del Software" ---
public class AboutView : IView
{
    private readonly InventoryManager _inventoryManager;

    public AboutView(InventoryManager manager)
    {
        _inventoryManager = manager;
    }

    public void Draw()
    {
        // 1. Dibuja el layout base con el panel de navegación
        DrawLayout("Acerca del software");

        // 2. Dibuja el contenido específico de esta vista
        int contentX = 27; // Ancho del panel izquierdo + 2
        int contentY = 3;

        Console.SetCursorPosition(contentX, contentY);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("/ Acerca del software");

        // Dibuja una caja para el contenido principal
        int boxY = contentY + 4;
        DrawBox(contentX, boxY, Console.WindowWidth - contentX - 2, 12, ConsoleColor.Green);

        // Escribe la información dentro de la caja
        string[] aboutLines = {
            "Sistema de Gestión de Inventario",
            "Versión 1.0.0",
            "",
            "Desarrollado por: JordyM",
            "Proyecto de Aplicación de Consola en .NET",
            "",
            "Copyright (c) 2025",
        };

        for (int i = 0; i < aboutLines.Length; i++)
        {
            Console.SetCursorPosition(contentX + 2, boxY + 2 + i);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(aboutLines[i]);
        }

        // Instrucciones de navegación
        string instructions = "Use las flechas ↑ ↓, ESC o Backspace para navegar.";
        Console.SetCursorPosition(Console.WindowWidth - instructions.Length - 3, Console.WindowHeight - 2);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(instructions);

        Console.ResetColor();
    }

    public IView HandleInput(ConsoleKeyInfo key)
    {
        // Esta vista solo necesita la navegación global.
        // Devuelve la vista que el helper determine o se queda aquí si no es una tecla de navegación.
        return NavigationHelper.HandleVerticalNavigation("Acerca del software", key, _inventoryManager) ?? this;
    }
}

