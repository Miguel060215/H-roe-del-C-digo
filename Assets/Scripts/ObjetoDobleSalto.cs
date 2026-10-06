using UnityEngine;

public class ObjetoDobleSalto : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip sonidoRecogido;
    [Range(0f, 1f)] public float volumen = 1f;

    [Header("Animación de Otix")]
    public string nombreAnimacionJugador = "RecoleccionItem2"; // El nombre de la animación en el Animator de Otix

    private bool yaRecogido = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaRecogido) return;

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            if (player != null)
            {
                yaRecogido = true;

                player.tieneDobleSalto = true;
                player.BloquarControles(2f);

                if (sonidoRecogido != null)
                {
                    AudioSource.PlayClipAtPoint(sonidoRecogido, transform.position, volumen);
                }

                // Buscamos el Animator en el jugador (Otix) y disparamos su animación
                Animator animJugador = collision.GetComponent<Animator>();
                if (animJugador != null)
                {
                    animJugador.Play(nombreAnimacionJugador);
                }

                // Desactivamos el coleccionable y lo destruimos en 2 segundos
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                Destroy(gameObject, 2f);
            }
        }
    }
}