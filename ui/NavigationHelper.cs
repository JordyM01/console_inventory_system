public static class NavigationHelper
{
    public static readonly string[] MenuItems = {
        "Inicio", "Agregar producto", "Mostrar productos",
        "Actualizar producto", "Eliminar producto", "Acerca del software", "Salir"
    };

    public static IView HandleVerticalNavigation(string currentViewName, ConsoleKeyInfo key, InventoryManager manager)
    {
        int currentIndex = Array.IndexOf(MenuItems, currentViewName);
        if (currentViewName == "MainMenuView") currentIndex = 0;

        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                int prevIndex = (currentIndex > 0) ? currentIndex - 1 : MenuItems.Length - 1;
                return GetViewByIndex(prevIndex, manager);

            case ConsoleKey.DownArrow:
                int nextIndex = (currentIndex < MenuItems.Length - 1) ? currentIndex + 1 : 0;
                return GetViewByIndex(nextIndex, manager);

                // --- CORRECCIÓN ---
                // Se ha eliminado la lógica de Backspace y Escape de este método.
                // Ahora, cada vista es responsable de manejar su propia tecla de "salida",
                // lo que permite un control más granular. Este helper solo se enfoca en la
                // navegación vertical del menú.
        }

        return null;
    }

    private static IView GetViewByIndex(int index, InventoryManager manager)
    {
        switch (index)
        {
            case 0: return new MainMenuView(manager);
            case 1: return new AddProductView(manager);
            case 2: return new ShowProductsView(manager);
            case 3: return new UpdateProductView(manager);
            case 4: return new DeleteProductView(manager);
            case 5: return new AboutView(manager);
            case 6: return null; // Salir
            default: return new MainMenuView(manager);
        }
    }
}


