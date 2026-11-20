using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona las acciones principales de navegacion y cierre de la aplicacion desde el menu principal.
/// </summary>
public class SistemaMenu : MonoBehaviour
{
    /// <summary>
    /// Carga la siguiente escena registrada en el perfil de compilacion respecto al indice de la escena actual.
    /// </summary>
    public void jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Finaliza la ejecucion de la aplicacion compilada.
    /// </summary>
    public void salir()
    {
        Debug.Log("Saliendo del juego"); // Permite verificar la llamada en la consola del editor, ya que Application.Quit() no cierra el modo Play
        Application.Quit();
    }
}