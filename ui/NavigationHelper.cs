/// <summary>
/// Clase estática con métodos centralizados para manejar la navegación del menú lateral.
/// </summary>
public static class NavigationHelper
{
    public static readonly string[] MenuItems = {
        "Inicio", "Agregar producto", "Mostrar productos",
        "Actualizar producto", "Eliminar producto", "Acerca del software", "Salir"
    };

    // CORRECCIÓN: Se cambió el nombre de 'HandleVerticalNavigation' a 'HandleMenuNavigation'
    // para que coincida con las llamadas desde las vistas.
    /// <summary>
    /// Procesa las teclas de flecha arriba/abajo para cambiar el índice de navegación.
    /// </summary>
    public static void HandleMenuNavigation(ConsoleKeyInfo key, ref int navigationIndex, InventoryManager manager)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                navigationIndex = (navigationIndex > 0) ? navigationIndex - 1 : MenuItems.Length - 1;
                break;

            case ConsoleKey.DownArrow:
                navigationIndex = (navigationIndex < MenuItems.Length - 1) ? navigationIndex + 1 : 0;
                break;
        }
    }

    // CORRECCIÓN: Se cambió el modificador de acceso de 'private' a 'public' para que
    // las vistas puedan llamar a este método.
    /// <summary>
    /// Devuelve una nueva instancia de una vista basada en su índice en el menú.
    /// </summary>
    public static IView GetViewByIndex(int index, InventoryManager manager)
    {
        return index switch
        {
            0 => new MainMenuView(manager),
            1 => new AddProductView(manager),
            2 => new ShowProductsView(manager),
            3 => new UpdateProductView(manager),
            4 => new DeleteProductView(manager),
            5 => new AboutView(manager),
            6 => null, // La opción "Salir" devuelve null para terminar la aplicación
            _ => new MainMenuView(manager) // Caso por defecto
        };
    }
}


