using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la pantalla negra y el mensaje introductorio al cargar un nivel por primera vez, evitando su repeticion tras reapariciones.
/// </summary>
public class CinematicaInicio : MonoBehaviour
{
    [Header("Referencias de Transición")]
    public Image pantallaNegra;
    public TextMeshProUGUI txtMensajeInicio;

    [Header("Tiempos de Animación")]
    public float tiempoEspera = 2.5f;
    public float tiempoDesvanecimiento = 1.5f;

    private static string ultimaEscenaReproducida = ""; // Almacena el nombre del ultimo nivel que mostro la introduccion para persistir entre recargas

    /// <summary>
    /// Verifica si la escena actual ya reprodujo su cinematica; en caso contrario, inicializa los elementos visuales y comienza la apertura.
    /// </summary>
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

    /// <summary>
    /// Mantiene el mensaje visible durante el tiempo configurado y reduce progresivamente la opacidad hasta ocultar el telon.
    /// </summary>
    private IEnumerator SecuenciaApertura()
    {
        yield return new WaitForSeconds(tiempoEspera);

        float t = 0;

        while (t < tiempoDesvanecimiento)
        {
            t += Time.deltaTime;
            float porcentaje = t / tiempoDesvanecimiento;
            // Interpola el canal alfa desde opaco (1) hasta totalmente transparente (0)
            float alphaAlpha = Mathf.Lerp(1f, 0f, porcentaje);

            if (txtMensajeInicio != null) txtMensajeInicio.color = new Color(1, 1, 1, alphaAlpha);
            if (pantallaNegra != null) pantallaNegra.color = new Color(0, 0, 0, alphaAlpha);

            yield return null;
        }

        if (pantallaNegra != null) pantallaNegra.gameObject.SetActive(false);
        if (txtMensajeInicio != null) txtMensajeInicio.gameObject.SetActive(false);
    }

    /// <summary>
    /// Limpia el registro estatico de escena reproducida para permitir que la cinematica vuelva a ejecutarse en una nueva partida.
    /// </summary>
    public static void ResetearCinematica()
    {
        ultimaEscenaReproducida = "";
    }
}