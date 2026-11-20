using UnityEngine;
using System.Collections;

/// <summary>
/// Controla la interaccion con la estacion de carga, ejecutando una secuencia animada que inmoviliza, oculta y restaura al maximo la salud del jugador.
/// </summary>
public class EstacionCarga : MonoBehaviour
{
    [Header("Configuración")]
    public float duracionAnimacion = 2f; // Tiempo en segundos que dura el ciclo de recarga antes de restaurar la salud y el control
    public KeyCode teclaInteraccion = KeyCode.Z;

    private bool jugadorCerca = false;
    private bool curando = false; // Bandera que evita reiniciar la rutina de curacion mientras ya se encuentra en ejecucion
    private GameObject jugadorObjeto;
    private Animator animatorBase;
    public GameObject indicadorTecla;

    [Header("Efectos de sonido")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoRecarga;

    /// <summary>
    /// Obtiene la referencia al componente Animator de la estacion al iniciar la escena.
    /// </summary>
    void Start()
    {
        animatorBase = GetComponent<Animator>();
    }

    /// <summary>
    /// Detecta la pulsacion de la tecla de interaccion cuando el jugador esta dentro del rango y no hay una recarga activa.
    /// </summary>
    void Update()
    {
        if (jugadorCerca && !curando && Input.GetKeyDown(teclaInteraccion))
        {
            StartCoroutine(RutinaCuracion());
            if (fuenteAudio != null && sonidoRecarga != null)
            {
                fuenteAudio.PlayOneShot(sonidoRecarga);
            }
        }
    }

    /// <summary>
    /// Muestra el indicador visual y almacena la referencia del jugador al ingresar en el area de la estacion.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            indicadorTecla.SetActive(true);
            jugadorCerca = true;
            jugadorObjeto = collision.gameObject;
        }
    }

    /// <summary>
    /// Oculta el indicador visual y limpia la referencia del jugador al abandonar el area de interaccion.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            indicadorTecla.SetActive(false);
            jugadorCerca = false;
            jugadorObjeto = null;
        }
    }

    /// <summary>
    /// Ejecuta la secuencia completa de recarga: detiene y oculta al jugador, activa la animacion de la base, restaura la salud y devuelve el control.
    /// </summary>
    private IEnumerator RutinaCuracion()
    {
        curando = true;

        Player scriptPlayer = jugadorObjeto.GetComponent<Player>();
        SaludJugador scriptSalud = jugadorObjeto.GetComponent<SaludJugador>();
        SpriteRenderer spriteOtix = jugadorObjeto.GetComponent<SpriteRenderer>();
        Rigidbody2D rbOtix = jugadorObjeto.GetComponent<Rigidbody2D>();

        // Detiene la velocidad fisica y deshabilita el renderizado para simular que el personaje ingresa a la estacion
        if (scriptPlayer != null) scriptPlayer.puedeMoverse = false;
        if (rbOtix != null) rbOtix.linearVelocity = Vector2.zero;
        if (spriteOtix != null) spriteOtix.enabled = false;

        animatorBase.SetBool("estaCurando", true);

        yield return new WaitForSeconds(duracionAnimacion);

        if (scriptSalud != null) scriptSalud.CurarAlMaximo();

        animatorBase.SetBool("estaCurando", false);

        // Restaura la visibilidad del sprite y habilita nuevamente el desplazamiento del jugador
        if (spriteOtix != null) spriteOtix.enabled = true;
        if (scriptPlayer != null) scriptPlayer.puedeMoverse = true;

        curando = false;
    }
}