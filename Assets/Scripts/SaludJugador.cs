using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SaludJugador : MonoBehaviour
{
    public int saludMaxima = 5;
    private int saludActual;

    [Header("Elementos Visuales Animados")]
    public Animator[] vidasAnimacion;

    [Header("Invencibilidad")]
    public float tiempoInvencibilidad = 2f;
    private bool esInvencible = false;
    public SpriteRenderer spriteRenderer;

    private Player scriptPlayer;

    void Start()
    {
        scriptPlayer = GetComponent<Player>();
        saludActual = saludMaxima;
        ActualizarVidasUI();
    }


    public void RecibirDaño(int cantidadDano)
    {
        if (esInvencible) return;

        saludActual -= cantidadDano;

        if (saludActual < 0) saludActual = 0;

        ActualizarVidasUI();

        if (saludActual == 0) Muerte();
        else StartCoroutine(RutinaInvencibilidad());
    }

    public void RecibirDannoTrampa(int cantidadDano)
    {
        if (esInvencible) return;

        // lo regresamos a la ultima zona segura q toco y le blokeamos el movimiento un rato
        Player scriptPlayer = GetComponent<Player>();
        if (scriptPlayer != null)
        {
            transform.position = scriptPlayer.ultimaPosicionSegura;
            scriptPlayer.BloquarControles(0.1f);
        }

        saludActual -= cantidadDano;

        if (saludActual < 0) saludActual = 0;

        ActualizarVidasUI();

        if (saludActual == 0) Muerte();
        else StartCoroutine(RutinaInvencibilidad());
    }

    // funcion extra para bajar vida y actulizar interfaz por si lo ocupo despues sin repetir codigo
    private void AplicarDannoLogica(int cantidadDano)
    {
        saludActual -= cantidadDano;
        if (saludActual < 0)
        {
            saludActual = 0;
        }
        ActualizarVidasUI();
        if (saludActual == 0)
        {
            Muerte();
        }
        else
        {
            StartCoroutine(RutinaInvencibilidad());
        }
    }

    void ActualizarVidasUI()
    {
        for (int i = 0; i < vidasAnimacion.Length; i++)
        {
            if (i < saludActual)
            {
                vidasAnimacion[i].SetBool("estaLlena", true);
            }
            else
            {
                vidasAnimacion[i].SetBool("estaLlena", false);
            }
        }
    }

    void Muerte()
    {
        Debug.Log("Otix ha muerto");
        // Despues programo que es lo que pasara al morir el personaje
    }

    // hace q el perosnaje parpadee y no reciba daño por un tiempo
    private IEnumerator RutinaInvencibilidad()
    {
        esInvencible = true;

        Color colorOriginal = spriteRenderer.color;

        Color colorTransparente = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.5f);

        float tiempoPasado = 0f;

        // un siklo q cambia el color a transparente y normal rapido
        while (tiempoPasado < tiempoInvencibilidad)
        {
            spriteRenderer.color = colorTransparente;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = colorOriginal;
            yield return new WaitForSeconds(0.1f);

            tiempoPasado += 0.2f;
        }

        spriteRenderer.color = colorOriginal;
        esInvencible = false;
    }
}