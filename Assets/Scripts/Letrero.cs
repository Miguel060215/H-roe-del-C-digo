using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Muestra y oculta progresivamente un mensaje visual en pantalla mediante transiciones de opacidad cuando el jugador entra o sale del area de un letrero.
/// </summary>
public class Letrero : MonoBehaviour
{
    [Header("Configuracion del letreto")]
    public Sprite mensajeSprite;
    public Image imagenPantalla;
    public float duracionFade = 1f;

    private Coroutine rutinaFadeActual; // Almacena la referencia de la corrutina activa para detenerla si el jugador entra y sale rapidamente

    /// <summary>
    /// Asigna el sprite del mensaje al componente UI e inicia el desvanecimiento de entrada al detectar al jugador.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (mensajeSprite != null)
            {
                imagenPantalla.sprite = mensajeSprite;
            }
            if (rutinaFadeActual != null)
            {
                StopCoroutine(rutinaFadeActual);
            }
            rutinaFadeActual = StartCoroutine(EfectoFade(1.5f));
        }
    }

    /// <summary>
    /// Inicia el desvanecimiento de salida al salir del area, o restablece el alfa directamente a cero si el objeto fue desactivado en la jerarquia.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Verifica que el GameObject siga activo porque Unity no permite iniciar corrutinas sobre objetos deshabilitados
            if (gameObject.activeInHierarchy)
            {
                if (rutinaFadeActual != null) StopCoroutine(rutinaFadeActual);
                rutinaFadeActual = StartCoroutine(EfectoFade(0f));
            }
            else
            {
                if (imagenPantalla != null)
                {
                    Color c = imagenPantalla.color;
                    imagenPantalla.color = new Color(c.r, c.g, c.b, 0f);
                }
            }
        }
    }

    /// <summary>
    /// Interpola gradualmente el canal alfa de la imagen en pantalla hasta alcanzar el valor objetivo durante el tiempo configurado.
    /// </summary>
    private IEnumerator EfectoFade(float alphaObjetivo)
    {
        Color colorActual = imagenPantalla.color;
        float tiempo = 0;
        float alphaInicial = colorActual.a;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            float nuevoAlpha = Mathf.Lerp(alphaInicial, alphaObjetivo, tiempo / duracionFade);

            imagenPantalla.color = new Color(colorActual.r, colorActual.g, colorActual.b, nuevoAlpha);
            yield return null;
        }
        imagenPantalla.color = new Color(colorActual.r, colorActual.g, colorActual.b, alphaObjetivo);
    }
}