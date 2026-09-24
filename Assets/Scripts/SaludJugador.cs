using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("Audio")]
    public AudioSource fuenteEfectos;
    public AudioClip clipDano;
    [Header("Secuencia de Muerte")]
    public AudioSource audioAmbiental;
    public AudioClip clipMuerte;
    public Image pantallaNegra;
    public SeguimientoCamara camaraScript; // Referencia al script de seguimiento de cámara

    void Start()
    {
        scriptPlayer = GetComponent<Player>();
        saludActual = saludMaxima;
        ActualizarVidasUI();
    }


    public void RecibirDaño(int cantidadDano)
    {
        if (esInvencible || saludActual <= 0) return;

        if (esInvencible) return;
        fuenteEfectos.PlayOneShot(clipDano,0.5f);

        saludActual -= cantidadDano;

        if (saludActual < 0) saludActual = 0;

        ActualizarVidasUI();

        if (saludActual == 0) Muerte();
        else StartCoroutine(RutinaInvencibilidad());
    }

    public void CurarAlMaximo() {
        saludActual = saludMaxima;
        ActualizarVidasUI();
    }

    public void RecibirDannoTrampa(int cantidadDano)
    {
        if (esInvencible) return;
        fuenteEfectos.PlayOneShot(clipDano, 0.5f);

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
        StartCoroutine(SecuenciaMuerte());
    }

    private IEnumerator SecuenciaMuerte() {
        //bloqueo el movimiento del personaje y lo detengo
        if (scriptPlayer != null) { 
            scriptPlayer.puedeMoverse = false;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) { 
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0f; // Evita que el personaje caiga mientras está muerto
            }
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) { 
                col.enabled = false;
            }
        }
        //hago que se incie la animacoon de muerte
        Animator anim = GetComponent<Animator>();
        if (anim != null) { 
            anim.SetTrigger("Muerte");
        }
        //apago la musica de fondo del nivel y pongo la de muerte de Otix
        if (audioAmbiental != null) {
            audioAmbiental.Stop();
        }
        if (scriptPlayer != null && clipMuerte != null)
        {
            scriptPlayer.fuenteCaminar.Stop();
            scriptPlayer.fuenteCaminar.clip = clipMuerte;
            scriptPlayer.fuenteCaminar.loop = true;
            scriptPlayer.fuenteCaminar.Play();
        }
        yield return new WaitForSeconds(3f);

        //se acerca la caamara al personaje muerto
        if (camaraScript != null) { 
            camaraScript.IniciarZoom(3f, 3f); //Si quiero ajujstar el zoom y la duracion, puedo cambiar los valores aqui
        }
        yield return new WaitForSeconds(3f);

        //se oscurece la pantalla
        if (pantallaNegra != null)
        {
            float tiempo = 0;
            float duracionFade = 1.5f;
            Color colorBase = pantallaNegra.color;

            while (tiempo < duracionFade) { 
                tiempo += Time.deltaTime;
                float alfa = Mathf.Lerp(0, 1, tiempo / duracionFade);
                pantallaNegra.color = new Color(colorBase.r, colorBase.g, colorBase.b, alfa);
                yield return null;
            }
        }
        //reinicio la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // hace q el perosnaje parpadee y no reciba daño por un tiempo
    private IEnumerator RutinaInvencibilidad()
    {
        esInvencible = true;

        Color colorOriginal = spriteRenderer.color;

        Color colorTransparente = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.5f);

        float tiempoPasado = 0f;

        // un siclo q cambia el color a transparente y normal rapido
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