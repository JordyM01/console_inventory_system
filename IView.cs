/// <summary>
/// Define en qué panel de la interfaz de usuario se encuentra el foco del usuario.
/// </summary>
public enum FocusState { Navigation, Content }

/// <summary>
/// Define el "contrato" que todas las clases de vista deben seguir.
/// </summary>
public interface IView
{
    void Draw(TuiRenderer renderer);
    IView? HandleInput(ConsoleKeyInfo key);
}


