using UnityEngine;

public class TerminalRetoFinal : MonoBehaviour
{

    [Header("Interfaz y referencias")]
    public GameObject panelSistemaArranque;
    public GameObject indicadorTecla;

    [Header("Configuracion")]
    public KeyCode teclaInter = KeyCode.Z;
    [Tooltip("Cantidad de cartas necesarias para activar el retoo")]
    public int cartasRequeridas = 1; //Despues cambio a 7

    private bool jugadorEnRango = false;
    private bool cumpleRequisito = false;

    private bool menuAbierto = false;

    void Start()
    {
        if (indicadorTecla != null) indicadorTecla.SetActive(false);
        if (panelSistemaArranque != null) panelSistemaArranque.SetActive(false);
    }

    void Update()
    {
        if (menuAbierto == false && jugadorEnRango && cumpleRequisito && Input.GetKeyDown(teclaInter))
        {
            AbrirSistemaReto();
        } else if (menuAbierto == true && Input.GetKeyDown(teclaInter))
        {
            CerrarSistemaReto();
        }
    }

    private void AbrirSistemaReto()
    {
        menuAbierto = true;
        indicadorTecla.SetActive(false);
        panelSistemaArranque.SetActive(true);
        Debug.Log("La tecla Z fue presionada y la funcion se ejecuto");
        Time.timeScale = 0f; //lo uso para pausar el jego
    }

    public void CerrarSistemaReto() {
        menuAbierto = false;
        panelSistemaArranque.SetActive(false);
        Time.timeScale = 1f; //lo uso para reanudar el juego
    }

    private bool JugadorTieneCartasSuficientes()
    {
        int cartasObtenidas = 0;

        for (int i = 0; i < ControladorMenu.cartasDesbloqueadas.Length; i++)
        {
            if (ControladorMenu.cartasDesbloqueadas[i] == true)
            {
                cartasObtenidas++;
            }
        }
        return cartasObtenidas >= cartasRequeridas;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = true;
            cumpleRequisito = JugadorTieneCartasSuficientes();
            if (cumpleRequisito && indicadorTecla != null)
            {
                indicadorTecla.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = false;
            if (indicadorTecla != null)
            {
                indicadorTecla.SetActive(false);
            }
        }
    }
}
