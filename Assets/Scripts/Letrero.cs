using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Letrero : MonoBehaviour
{
    [Header("Configuracion del letreto")]
    public Sprite mensajeSprite;
    public Image imagenPantalla;
    public float duracionFade = 1f;

    private Coroutine rutinaFadeActual;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) { 
            if(mensajeSprite != null)
            {
                imagenPantalla.sprite = mensajeSprite;
            }
            if (rutinaFadeActual != null) {
                StopCoroutine(rutinaFadeActual);
            }
            rutinaFadeActual = StartCoroutine(EfectoFade(1.5f));
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
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

    private IEnumerator EfectoFade(float alphaObjetivo) { 
        Color colorActual = imagenPantalla.color;
        float tiempo = 0;
        float alphaInicial = colorActual.a;

        while (tiempo < duracionFade) { 
            tiempo += Time.deltaTime;
            float nuevoAlpha = Mathf.Lerp(alphaInicial, alphaObjetivo, tiempo / duracionFade);

            imagenPantalla.color = new Color(colorActual.r, colorActual.g, colorActual.b, nuevoAlpha);
            yield return null;
        }
        imagenPantalla.color = new Color(colorActual.r, colorActual.g, colorActual.b, alphaObjetivo);
    }


}
