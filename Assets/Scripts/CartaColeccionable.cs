using UnityEngine;

public class CartaColeccionable : MonoBehaviour
{
    [Header("Configuracion")]
    public int idCarta; // es para identificar a cada una de las 7 u 8 cartas que voy a dar;e al jugador
    public GameObject indicadorTecla;

    private bool jugadorEnRango = false;
    private Player jugadorScript;
    private Animator jugadorAnim;


    // Update is called once per frame
    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.Z)) {
            RecogerCarta();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            jugadorEnRango = true;
            indicadorTecla.SetActive(true);//aparece la animacion de tecla

            jugadorScript = collision.GetComponent<Player>();
            jugadorAnim = collision.GetComponent<Animator>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            jugadorEnRango = false;
            indicadorTecla.SetActive(false);//desapacece la animacion de la tecla

            jugadorScript = null;
            jugadorAnim = null;
        }
    }

    private void RecogerCarta() {
        if (jugadorScript != null) {
            jugadorScript.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            jugadorScript.BloquarControles(2.5f);//aqui se bloquea los controles, el valor depende de cuanto dure mi animacion de otix obteniendo la carta

        }
        if (jugadorAnim != null) {
            jugadorAnim.SetFloat("Speed", 0f);
            jugadorAnim.SetTrigger("Recolectar");
        }

        Debug.Log("Carta " + idCarta + "recolectada. Falta abrir la interfaz");
        Destroy(gameObject);
    }
}
