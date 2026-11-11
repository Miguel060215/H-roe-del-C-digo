using UnityEngine;
using System.Collections;

public class TerminalRetoFinal : MonoBehaviour
{
    [Header("Interfaz y referencias")]
    public GameObject panelSistemaArranque;
    public GameObject indicadorTecla;

    [Header("Configuracion")]
    public KeyCode teclaInter = KeyCode.Z;
    [Tooltip("Cantidad de cartas necesarias para activar el retoo")]
    public int cartasRequeridas = 7; //Despues cambio a 7

    [Header("Animacion del Panel")]
    public RectTransform panelRect;
    public float duracionAnimacion = 0.4f;
    public Vector2 posicionCentro = new Vector2(0, 0);
    public Vector2 posicionAbajo = new Vector2(0, -1500);

    private bool jugadorEnRango = false;
    private bool cumpleRequisito = false;
    private bool menuAbierto = false;
    private Coroutine animacionActual;

    void Start()
    {
        if (indicadorTecla != null) indicadorTecla.SetActive(false);
        if (panelRect != null) panelRect.anchoredPosition = posicionAbajo;
        if (panelSistemaArranque != null) panelSistemaArranque.SetActive(false);
    }

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

    private void AbrirSistemaReto()
    {
        menuAbierto = true;
        indicadorTecla.SetActive(false);
        panelSistemaArranque.SetActive(true);
        Debug.Log("La tecla Z fue presionada y la funcion se ejecuto");
        Time.timeScale = 0f; //lo uso para pausar el jego
        animacionActual = StartCoroutine(AnimarPanel(posicionCentro, false));
    }

    public void CerrarSistemaReto()
    {
        menuAbierto = false;
        Time.timeScale = 1f; //lo uso para reanudar el juego

        if (jugadorEnRango && cumpleRequisito && indicadorTecla != null)
        {
            indicadorTecla.SetActive(true);
        }

        animacionActual = StartCoroutine(AnimarPanel(posicionAbajo, true));
    }

    private IEnumerator AnimarPanel(Vector2 posicionDestino, bool apagarAlTerminar)
    {
        Vector2 posicionInicial = panelRect.anchoredPosition;
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionAnimacion)
        {
            tiempoPasado += Time.unscaledDeltaTime;
            float porcentaje = tiempoPasado / duracionAnimacion;
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
            if (cumpleRequisito && indicadorTecla != null && !menuAbierto)
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