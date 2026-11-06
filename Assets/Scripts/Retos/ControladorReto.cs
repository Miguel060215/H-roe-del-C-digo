using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Baase de datos")]
    public ProblemaPython[] listaProblemas;

    private int problemaActualIndice = 0;
    private int aciertos = 0;
    private int opcionesSeleccionada = -1;

    [Header("UI elementos")]
    public GameObject panelBlanco;
    public CanvasGroup grupoInteractivo;

    void OnEnable()
    {
        aciertos = 0;
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
        /*if (opcionesSeleccionada == -1) {
            return;
        }

        imgFeedBack.gameObject.SetActive(true);
        ProblemaPython problema = listaProblemas[problemaActualIndice];

        if (opcionesSeleccionada == problema.indiceCorrecto)
        {
            imgFeedBack.sprite = problema.feedBackAcierto;
            aciertos++;
            ActualizarContadorAciertos();
            //despues agrego la logica para cambiar de problema
        }
        else {
            imgFeedBack.sprite = problema.feedBackError;
        }*/
        if (opcionesSeleccionada == -1) return;

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
}