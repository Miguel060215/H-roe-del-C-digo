using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la recoleccion del item que desbloquea la habilidad de doble salto del jugador, activando su animacion y efectos sonoros correspondientes.
/// </summary>
public class ObjetoDobleSalto : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip sonidoRecogido;
    [Range(0f, 1f)] public float volumen = 1f;

    [Header("Animación de Otix")]
    public string nombreAnimacionJugador = "RecoleccionItem2"; // Nombre exacto del estado en el Animator del jugador que se reproducira al obtener el objeto

    private bool yaRecogido = false; // Bandera que previene ejecuciones multiples durante el retardo previo a la destruccion del objeto

    /// <summary>
    /// Elimina el objeto de la escena al reaparecer si el jugador ya lo habia recolectado previamente en este nivel.
    /// </summary>
    private void Start()
    {
        if (Player.tieneDobleSalto && Player.escenaDobleSalto == SceneManager.GetActiveScene().name)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Detecta el contacto con el jugador, habilita permanentemente el doble salto, bloquea temporalmente los controles y oculta el objeto antes de destruirlo.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaRecogido) return;

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            if (player != null)
            {
                yaRecogido = true;

                Player.tieneDobleSalto = true;
                Player.escenaDobleSalto = SceneManager.GetActiveScene().name;
                player.BloquarControles(2f);

                if (sonidoRecogido != null)
                {
                    AudioSource.PlayClipAtPoint(sonidoRecogido, transform.position, volumen);
                }

                Animator animJugador = collision.GetComponent<Animator>();
                if (animJugador != null)
                {
                    animJugador.Play(nombreAnimacionJugador);
                }

                // Desactiva los componentes visuales y fisicos de inmediato mientras transcurre el tiempo antes de eliminar el GameObject
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                Destroy(gameObject, 2f);
            }
        }
    }
}