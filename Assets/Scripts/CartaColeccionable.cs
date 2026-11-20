using UnityEngine;

/// <summary>
/// Gestiona la interaccion y recoleccion de una carta coleccionable en la escena, actualizando el HUD, el menu y el estado del jugador.
/// </summary>
public class CartaColeccionable : MonoBehaviour
{
    [Header("Configuracion")]
    public int idCarta; // Identificador unico que vincula esta carta con su ranura en el HUD y en el menu
    public GameObject indicadorTecla;
    public AudioClip sonidoRecoleccion;

    private bool jugadorEnRango = false;
    private Player jugadorScript;
    private Animator jugadorAnim;

    /// <summary>
    /// Verifica al iniciar la escena si la carta ya fue obtenida previamente para eliminarla y evitar duplicados.
    /// </summary>
    void Start()
    {
        if (HUDCartas.cartasRecolectadas.Contains(idCarta))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Detecta si el jugador presiona la tecla de interaccion mientras se encuentra dentro del area de la carta.
    /// </summary>
    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.Z))
        {
            RecogerCarta();
        }
    }

    /// <summary>
    /// Habilita la posibilidad de interaccion, muestra el indicador visual y obtiene las referencias del jugador al entrar en contacto.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = true;
            indicadorTecla.SetActive(true);

            jugadorScript = collision.GetComponent<Player>();
            jugadorAnim = collision.GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Deshabilita la interaccion, oculta el indicador visual y libera las referencias del jugador al salir del area.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = false;
            indicadorTecla.SetActive(false);

            jugadorScript = null;
            jugadorAnim = null;
        }
    }

    /// <summary>
    /// Ejecuta la secuencia de recoleccion: inmoviliza al jugador, activa su animacion, actualiza el HUD y destruye el objeto.
    /// </summary>
    private void RecogerCarta()
    {
        if (jugadorScript != null)
        {
            jugadorScript.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            jugadorScript.BloquarControles(2.5f); // Restringe el movimiento temporalmente para permitir que finalice la animacion de recoleccion
        }
        if (jugadorAnim != null)
        {
            jugadorAnim.SetFloat("Speed", 0f);
            jugadorAnim.SetTrigger("Recolectar");
        }

        ControladorMenu menu = FindAnyObjectByType<ControladorMenu>();
        if (menu != null)
        {
            menu.AbrirMenuConRetraso(idCarta, 2f);
        }
        //Debug.Log("Carta " + idCarta + "recolectada. Falta abrir la interfaz");

        HUDCartas hud = FindAnyObjectByType<HUDCartas>();
        if (hud != null)
        {
            hud.ActivarIcono(idCarta);
        }

        if (sonidoRecoleccion != null)
        {
            // PlayClipAtPoint crea una fuente de audio temporal en la escena para que el sonido no se corte al destruir este objeto
            AudioSource.PlayClipAtPoint(sonidoRecoleccion, Camera.main.transform.position);
        }

        Destroy(gameObject);
    }
}