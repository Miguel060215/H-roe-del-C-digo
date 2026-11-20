using UnityEngine;
using System.Collections;

/// <summary>
/// Controla la interaccion con la terminal del reto final, validando el requisito de cartas y gestionando la animacion de apertura y cierre del panel.
/// </summary>
public class TerminalRetoFinal : MonoBehaviour
{
    [Header("Interfaz y referencias")]
    public GameObject panelSistemaArranque;
    public GameObject indicadorTecla;

    [Header("Configuracion")]
    public KeyCode teclaInter = KeyCode.Z;
    [Tooltip("Cantidad de cartas necesarias para activar el retoo")]
    public int cartasRequeridas = 7;

    [Header("Animacion del Panel")]
    public RectTransform panelRect;
    public float duracionAnimacion = 0.4f;
    public Vector2 posicionCentro = new Vector2(0, 0);
    public Vector2 posicionAbajo = new Vector2(0, -1500);

    private bool jugadorEnRango = false;
    private bool cumpleRequisito = false;
    private bool menuAbierto = false;
    private Coroutine animacionActual; // Almacena la corrutina activa para evitar solapamientos al presionar la tecla repetidamente

    /// <summary>
    /// Inicializa el estado oculto del indicador y posiciona el panel fuera de pantalla al comenzar la escena.
    /// </summary>
    void Start()
    {
        if (indicadorTecla != null) indicadorTecla.SetActive(false);
        if (panelRect != null) panelRect.anchoredPosition = posicionAbajo;
        if (panelSistemaArranque != null) panelSistemaArranque.SetActive(false);
    }

    /// <summary>
    /// Detecta la pulsacion de la tecla de interaccion para abrir o cerrar el panel si no hay una animacion en curso.
    /// </summary>
    void Update()
    {
        if (menuAbierto == false && jugadorEnRango && cumpleRequisito && Input.GetKeyDown(teclaInter) && animacionActual == null)
        {
            AbrirSistemaReto();
        }
        else if (menuAbierto == true && Input.GetKeyDown(teclaInter) && animacionActual == null)
        {
            CerrarSistemaReto();
        }
    }

    /// <summary>
    /// Activa el panel del reto, pausa el tiempo del juego y despliega la interfaz hacia el centro de la pantalla.
    /// </summary>
    private void AbrirSistemaReto()
    {
        menuAbierto = true;
        indicadorTecla.SetActive(false);
        panelSistemaArranque.SetActive(true);
        Debug.Log("La tecla Z fue presionada y la funcion se ejecuto");
        Time.timeScale = 0f; // Pausa la ejecucion fisica y temporal del nivel
        animacionActual = StartCoroutine(AnimarPanel(posicionCentro, false));
    }

    /// <summary>
    /// Reanuda el tiempo del juego, restaura el indicador si el jugador sigue cerca y desliza el panel hacia abajo.
    /// </summary>
    public void CerrarSistemaReto()
    {
        menuAbierto = false;
        Time.timeScale = 1f; // Restaura la velocidad normal del juego

        if (jugadorEnRango && cumpleRequisito && indicadorTecla != null)
        {
            indicadorTecla.SetActive(true);
        }

        animacionActual = StartCoroutine(AnimarPanel(posicionAbajo, true));
    }

    /// <summary>
    /// Desplaza suavemente el RectTransform del panel entre dos coordenadas ignorando la escala de tiempo pausada.
    /// </summary>
    private IEnumerator AnimarPanel(Vector2 posicionDestino, bool apagarAlTerminar)
    {
        Vector2 posicionInicial = panelRect.anchoredPosition;
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionAnimacion)
        {
            // Se utiliza unscaledDeltaTime para permitir el movimiento mientras Time.timeScale es 0
            tiempoPasado += Time.unscaledDeltaTime;
            float porcentaje = tiempoPasado / duracionAnimacion;
            // SmoothStep aplica aceleracion y desaceleracion suave a la interpolacion
            float curva = Mathf.SmoothStep(0f, 1f, porcentaje);
            panelRect.anchoredPosition = Vector2.Lerp(posicionInicial, posicionDestino, curva);
            yield return null;
        }

        panelRect.anchoredPosition = posicionDestino;
        animacionActual = null;

        if (apagarAlTerminar && panelSistemaArranque != null)
        {
            panelSistemaArranque.SetActive(false);
        }
    }

    /// <summary>
    /// Recorre el arreglo global de cartas desbloqueadas y verifica si alcanza el minimo requerido.
    /// </summary>
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

    /// <summary>
    /// Detecta el ingreso del jugador al area de la terminal y evalua si cumple con el requisito de cartas.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = true;
            cumpleRequisito = JugadorTieneCartasSuficientes();
            if (cumpleRequisito && indicadorTecla != null && !menuAbierto)
            {
                indicadorTecla.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Detecta la salida del jugador del area de interaccion y oculta el indicador visual.
    /// </summary>
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