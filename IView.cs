/// <summary>
/// Define en qué panel de la interfaz de usuario se encuentra el foco del usuario.
/// </summary>
public enum FocusState { Navigation, Content }

/// <summary>
/// Interfaz para todas las Vistas.
/// Define la estructura que toda clase de vista debe seguir.
/// </summary>
public interface IView
{
    void Draw();
    IView HandleInput(ConsoleKeyInfo key);
}



