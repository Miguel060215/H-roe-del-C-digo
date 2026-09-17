using UnityEngine;
using UnityEngine.SceneManagement;

public class SistemaMenu : MonoBehaviour
{
    public  void jugar() { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void salir() { 
        Debug.Log("Saliendo del juego");// ES para saber si funciona esta funcion
        Application.Quit();
    }
}
