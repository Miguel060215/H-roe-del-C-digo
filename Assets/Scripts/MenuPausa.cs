using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla el estado de pausa del juego, la navegacion hacia el menu principal o reinicio del nivel y la limpieza de los datos estaticos persistentes.
/// </summary>
public class MenuPausa : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelPausa;

    [Header("Configuracion")]
    public KeyCode teclaPausa = KeyCode.P;
    public string nombreEscenaMenu = "Menu";

    private bool juegoPausado = false;

    /// <summary>
    /// Asegura que la interfaz del menu de pausa permanezca oculta al iniciar la escena.
    /// </summary>
    void Start()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
    }

    /// <summary>
    /// Escucha la pulsacion de la tecla configurada para alternar entre pausar o reanudar la partida.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(teclaPausa))
        {
            if (juegoPausado)
            {
                ReanudarJuego();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    /// <summary>
    /// Activa el panel de pausa y detiene el flujo temporal del juego si este no se encuentra pausado previamente por otra interfaz.
    /// </summary>
    public void PausarJuego()
    {
        // Evita abrir el menu de pausa si otro sistema (como la terminal de retos) ya detuvo Time.timeScale
        if (Time.timeScale == 0f) return;

        juegoPausado = true;
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Oculta el panel de pausa y restablece la velocidad normal de ejecucion del juego.
    /// </summary>
    public void ReanudarJuego()
    {
        juegoPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Restaura la escala de tiempo, limpia el progreso acumulado del nivel y recarga la escena actual.
    /// </summary>
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        ResetearDatosNivel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Restaura el tiempo, limpia todos los registros estaticos de progreso y cinematica, y carga la escena del menu principal.
    /// </summary>
    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        ResetearDatosNivel();
        CinematicaInicio.ResetearCinematica();
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    /// <summary>
    /// Reinicia las variables estaticas globales de cartas desbloqueadas, iconos del HUD y puntos de control del jugador.
    /// </summary>
    private void ResetearDatosNivel()
    {
        if (ControladorMenu.cartasDesbloqueadas != null)
        {
            for (int i = 0; i < ControladorMenu.cartasDesbloqueadas.Length; i++)
            {
                ControladorMenu.cartasDesbloqueadas[i] = false;
            }
        }

        HUDCartas.cartasRecolectadas.Clear();

        Player.hayPuntoGuardado = false;
        Player.puntoReaparicion = Vector3.zero;
        Player.tieneDobleSalto = false;
        Player.escenaDobleSalto = "";
    }
}