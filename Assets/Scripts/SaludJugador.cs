using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Administra los puntos de vida del jugador, los periodos de invencibilidad temporal, la actualizacion del HUD de salud y la secuencia cinematografica de muerte y reinicio.
/// </summary>
public class SaludJugador : MonoBehaviour
{
    public int saludMaxima = 5;
    private int saludActual;

    [Header("Elementos Visuales Animados")]
    public Animator[] vidasAnimacion;

    [Header("Invencibilidad")]
    public float tiempoInvencibilidad = 2f;
    private bool esInvencible = false; // Bandera que ignora impactos adicionales mientras dura el parpadeo de inmunidad
    public SpriteRenderer spriteRenderer;

    private Player scriptPlayer;

    [Header("Audio")]
    public AudioSource fuenteEfectos;
    public AudioClip clipDano;

    [Header("Secuencia de Muerte")]
    public AudioSource audioAmbiental;
    public AudioClip clipMuerte;
    public Image pantallaNegra;
    public SeguimientoCamara camaraScript;
    public bool estaMuerto = false;

    /// <summary>
    /// Inicializa el estado de vida al maximo, obtiene la referencia del controlador del jugador y sincroniza los indicadores visuales en pantalla.
    /// </summary>
    void Start()
    {
        estaMuerto = false;
        scriptPlayer = GetComponent<Player>();
        saludActual = saludMaxima;
        ActualizarVidasUI();
    }

    /// <summary>
    /// Reduce la salud tras un impacto directo si el jugador no es invencible, actualiza la interfaz y activa la muerte o la rutina de inmunidad.
    /// </summary>
    public void RecibirDaño(int cantidadDano)
    {
        if (estaMuerto || esInvencible || saludActual <= 0) return;

        fuenteEfectos.PlayOneShot(clipDano, 0.5f);
        saludActual -= cantidadDano;
        if (saludActual < 0) saludActual = 0;
        ActualizarVidasUI();

        if (saludActual == 0)
        {
            estaMuerto = true;
            Muerte();
        }
        else StartCoroutine(RutinaInvencibilidad());
    }

    /// <summary>
    /// Restaura la salud actual al valor maximo permitido y actualiza los indicadores del HUD.
    /// </summary>
    public void CurarAlMaximo()
    {
        saludActual = saludMaxima;
        ActualizarVidasUI();
    }

    /// <summary>
    /// Aplica dano por trampa, reubica al jugador en su ultima posicion segura registrada, restringe brevemente el control y gestiona el estado resultante.
    /// </summary>
    public void RecibirDannoTrampa(int cantidadDano)
    {
        if (estaMuerto || esInvencible) return;
        fuenteEfectos.PlayOneShot(clipDano, 0.5f);

        Player scriptPlayer = GetComponent<Player>();
        if (scriptPlayer != null)
        {
            transform.position = scriptPlayer.ultimaPosicionSegura;
            scriptPlayer.BloquarControles(0.1f);
        }

        saludActual -= cantidadDano;

        if (saludActual < 0) saludActual = 0;

        ActualizarVidasUI();

        if (saludActual == 0)
        {
            estaMuerto = true;
            Muerte();
        }
        else StartCoroutine(RutinaInvencibilidad());
    }

    /// <summary>
    /// Centraliza la deduccion de puntos de salud, la actualizacion del HUD y la evaluacion de derrota o invencibilidad.
    /// </summary>
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

    /// <summary>
    /// Sincroniza el estado booleano de cada Animator de vida en la interfaz segun la cantidad de salud actual.
    /// </summary>
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

    /// <summary>
    /// Inicia el flujo de derrota del jugador ejecutando la corrutina de secuencia de muerte.
    /// </summary>
    void Muerte()
    {
        Debug.Log("Otix ha muerto");
        StartCoroutine(SecuenciaMuerte());
    }

    /// <summary>
    /// Inmoviliza al personaje, reproduce la animacion y el audio de derrota, acerca la camara, oscurece la pantalla y recarga la escena actual.
    /// </summary>
    private IEnumerator SecuenciaMuerte()
    {
        if (scriptPlayer != null)
        {
            scriptPlayer.puedeMoverse = false;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f; // Anula la gravedad para mantener el cuerpo estatico durante la animacion de derrota
            }
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
            anim.SetFloat("SpeedY", 0f);
            anim.SetBool("enSuelo", true);

            anim.SetTrigger("Muerte");
        }

        if (audioAmbiental != null)
        {
            audioAmbiental.Stop();
        }
        if (scriptPlayer != null && clipMuerte != null)
        {
            // Reutiliza el AudioSource de pasos del jugador para reproducir en bucle la melodia de derrota
            scriptPlayer.fuenteCaminar.Stop();
            scriptPlayer.fuenteCaminar.clip = clipMuerte;
            scriptPlayer.fuenteCaminar.loop = true;
            scriptPlayer.fuenteCaminar.Play();
        }
        yield return new WaitForSeconds(3f);

        if (camaraScript != null)
        {
            camaraScript.IniciarZoom(3f, 3f);
        }
        yield return new WaitForSeconds(3f);

        if (pantallaNegra != null)
        {
            float tiempo = 0;
            float duracionFade = 1.5f;
            Color colorBase = pantallaNegra.color;

            while (tiempo < duracionFade)
            {
                tiempo += Time.deltaTime;
                float alfa = Mathf.Lerp(0, 1, tiempo / duracionFade);
                pantallaNegra.color = new Color(colorBase.r, colorBase.g, colorBase.b, alfa);
                yield return null;
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Activa la inmunidad temporal y alterna la transparencia del sprite en intervalos de 0.1 segundos para dar retroalimentacion visual al jugador.
    /// </summary>
    private IEnumerator RutinaInvencibilidad()
    {
        esInvencible = true;

        Color colorOriginal = spriteRenderer.color;
        Color colorTransparente = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.5f);

        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoInvencibilidad)
        {
            spriteRenderer.color = colorTransparente;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = colorOriginal;
            yield return new WaitForSeconds(0.1f);

            // Suma 0.2f por iteracion correspondiente a los dos tiempos de espera de 0.1 segundos
            tiempoPasado += 0.2f;
        }

        spriteRenderer.color = colorOriginal;
        esInvencible = false;
    }
}