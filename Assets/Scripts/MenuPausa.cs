using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelPausa;

    [Header("Configuracion")]
    public KeyCode teclaPausa = KeyCode.P;
    public string nombreEscenaMenu = "Menu";

    private bool juegoPausado = false;

    void Start()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
    }

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

    public void PausarJuego()
    {
        if (Time.timeScale == 0f) return;

        juegoPausado = true;
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        ResetearDatosNivel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        ResetearDatosNivel();
        CinematicaInicio.ResetearCinematica();
        SceneManager.LoadScene(nombreEscenaMenu);
    }

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
    }
}