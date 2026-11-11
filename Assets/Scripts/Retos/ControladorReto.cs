using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ProblemaPython
{
    public Sprite imagenProblema;
    public Sprite[] imagenesOpciones = new Sprite[4];

    [Header("FeedBacks especificos")]
    [Tooltip("Imagenes de errores de cada reto, la respuesta correcta quedara vaciaa")]
    public Sprite[] feedbacksError = new Sprite[4];

    [Tooltip("El numero de la opcion correcta (0-3)")]
    public int indiceCorrecto;
    public Sprite feedBackAcierto;
}

public class ControladorReto : MonoBehaviour
{
    [Header("Referencias")]
    public Image imgProblemaActual;
    public Image[] botonesOpciones = new Image[4];
    public Image imgFeedBack;

    [Header("Controlador de aciertos (imagenes)")]
    public Image imgContadorAciertos;
    public Sprite[] spritesAciertos;

    [Header("Transicion de Nivel")]
    public Image pantallaNegra;
    public TextMeshProUGUI txtMensaje1;
    public string nombreSiguienteEscena = "Nivel2";

    [Header("Baase de datos")]
    public ProblemaPython[] listaProblemas;

    private int problemaActualIndice = 0;
    private int aciertos = 0;
    private int opcionesSeleccionada = -1;
    private bool enTransicion = false;

    [Header("UI elementos")]
    public GameObject panelBlanco;
    public CanvasGroup grupoInteractivo;

    void OnEnable()
    {
        aciertos = 0;
        enTransicion = false;

        if (pantallaNegra != null) pantallaNegra.color = new Color(0, 0, 0, 0);
        if (txtMensaje1 != null) txtMensaje1.color = new Color(1, 1, 1, 0);
        if (pantallaNegra != null) pantallaNegra.gameObject.SetActive(false);

        CargarProblemas(0);
    }

    public void CargarProblemas(int indice)
    {
        problemaActualIndice = indice;
        opcionesSeleccionada = -1;
        imgFeedBack.gameObject.SetActive(false);

        ProblemaPython problema = listaProblemas[indice];

        imgProblemaActual.sprite = problema.imagenProblema;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            botonesOpciones[i].sprite = problema.imagenesOpciones[i];
            botonesOpciones[i].color = Color.white;
        }

        ActualizarContadorAciertos();
    }

    public void SeleccionarOpcion(int indiceBoton)
    {
        if (enTransicion) return;

        opcionesSeleccionada = indiceBoton;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (i == indiceBoton)
            {
                botonesOpciones[i].color = Color.white;
            }
            else
            {
                botonesOpciones[i].color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    public void VerificarRespuesta()
    {
        if (opcionesSeleccionada == -1 || enTransicion) return;

        imgFeedBack.gameObject.SetActive(true);
        ProblemaPython problema = listaProblemas[problemaActualIndice];

        if (opcionesSeleccionada == problema.indiceCorrecto)
        {
            imgFeedBack.sprite = problema.feedBackAcierto;

            if (imgContadorAciertos.sprite != spritesAciertos[2])
            {
                aciertos++;
                ActualizarContadorAciertos();
            }

            if (aciertos >= 3)
            {
                enTransicion = true;
                StartCoroutine(SecuenciaVictoria());
                return;
            }
        }
        else
        {
            imgFeedBack.sprite = problema.feedbacksError[opcionesSeleccionada];
        }

        StartCoroutine(RutinaCambioDeReto());
    }

    public void ActualizarContadorAciertos()
    {
        if (aciertos == 0)
        {
            imgContadorAciertos.gameObject.SetActive(false);
        }
        else if (aciertos > 0 && aciertos <= spritesAciertos.Length)
        {
            imgContadorAciertos.gameObject.SetActive(true);
            imgContadorAciertos.sprite = spritesAciertos[aciertos - 1];
        }
    }

    public void BotonOtroReto()
    {
        if (enTransicion) return;
        StartCoroutine(RutinaCambioDeReto());
    }

    private IEnumerator RutinaCambioDeReto()
    {
        if (grupoInteractivo != null) grupoInteractivo.interactable = false;
        if (panelBlanco != null) panelBlanco.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        if (panelBlanco != null) panelBlanco.SetActive(false);
        if (grupoInteractivo != null) grupoInteractivo.interactable = true;

        int siguienteIndice = problemaActualIndice + 1;
        if (siguienteIndice < listaProblemas.Length)
        {
            CargarProblemas(siguienteIndice);
        }
    }

    private IEnumerator SecuenciaVictoria()
    {
        pantallaNegra.gameObject.SetActive(true);

        float t = 0;
        while (t < 1.5f)
        {
            t += Time.unscaledDeltaTime;
            pantallaNegra.color = new Color(0, 0, 0, t / 1.5f);
            yield return null;
        }

        t = 0;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            txtMensaje1.color = new Color(1, 1, 1, t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(5.5f);

        t = 0;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            txtMensaje1.color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreSiguienteEscena);
    }
}