using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; 

public class CinematicaInicio : MonoBehaviour
{
    [Header("Referencias de Transición")]
    public Image pantallaNegra;
    public TextMeshProUGUI txtMensajeInicio;

    [Header("Tiempos de Animación")]
    public float tiempoEspera = 2.5f;
    public float tiempoDesvanecimiento = 1.5f;

    private static string ultimaEscenaReproducida = "";

    void Start()
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        if (ultimaEscenaReproducida == escenaActual)
        {
            if (pantallaNegra != null) pantallaNegra.gameObject.SetActive(false);
            if (txtMensajeInicio != null) txtMensajeInicio.gameObject.SetActive(false);
            return;
        }

        ultimaEscenaReproducida = escenaActual;

        if (pantallaNegra != null)
        {
            pantallaNegra.gameObject.SetActive(true);
            pantallaNegra.color = new Color(0, 0, 0, 1);
        }

        if (txtMensajeInicio != null)
        {
            txtMensajeInicio.gameObject.SetActive(true);
            txtMensajeInicio.color = new Color(1, 1, 1, 1);
        }

        StartCoroutine(SecuenciaApertura());
    }

    private IEnumerator SecuenciaApertura()
    {
        yield return new WaitForSeconds(tiempoEspera);

        float t = 0;

        while (t < tiempoDesvanecimiento)
        {
            t += Time.deltaTime;
            float porcentaje = t / tiempoDesvanecimiento;
            float alphaAlpha = Mathf.Lerp(1f, 0f, porcentaje);

            if (txtMensajeInicio != null) txtMensajeInicio.color = new Color(1, 1, 1, alphaAlpha);
            if (pantallaNegra != null) pantallaNegra.color = new Color(0, 0, 0, alphaAlpha);

            yield return null;
        }

        if (pantallaNegra != null) pantallaNegra.gameObject.SetActive(false);
        if (txtMensajeInicio != null) txtMensajeInicio.gameObject.SetActive(false);
    }
}