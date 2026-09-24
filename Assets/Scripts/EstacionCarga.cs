using UnityEngine;
using System.Collections;

public class EstacionCarga : MonoBehaviour
{
    [Header("Configuración")]
    public float duracionAnimacion = 2f; //Ajusto aqui cuando vea la duracion de mi animacion de subir vida al maximoo
    public KeyCode teclaInteraccion = KeyCode.Z; //la tecla para interactuar con la base de curacon

    private bool jugadorCerca = false;
    private bool curando = false;
    private GameObject jugadorObjeto;
    private Animator animatorBase;

    void Start()
    {
        animatorBase = GetComponent<Animator>();
    }

    void Update()
    {
        
        if (jugadorCerca && !curando && Input.GetKeyDown(teclaInteraccion))
        {
            StartCoroutine(RutinaCuracion());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
            jugadorObjeto = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
            jugadorObjeto = null;
        }
    }

    private IEnumerator RutinaCuracion()
    {
        curando = true;

        // 1. Obtener los componentes de Otix
        Player scriptPlayer = jugadorObjeto.GetComponent<Player>();
        SaludJugador scriptSalud = jugadorObjeto.GetComponent<SaludJugador>();
        SpriteRenderer spriteOtix = jugadorObjeto.GetComponent<SpriteRenderer>();
        Rigidbody2D rbOtix = jugadorObjeto.GetComponent<Rigidbody2D>();

        // 2. Frenar a Otix, bloquear sus controles y volverlo INVISIBLE
        if (scriptPlayer != null) scriptPlayer.puedeMoverse = false;
        if (rbOtix != null) rbOtix.linearVelocity = Vector2.zero; // Frenado en seco
        if (spriteOtix != null) spriteOtix.enabled = false;

        // 3. Activar la animación de la base curando
        animatorBase.SetBool("estaCurando", true);

        // 4. Esperar los segundos que dura tu animación
        yield return new WaitForSeconds(duracionAnimacion);

        // 5. Rellenar las vidas al 100%
        if (scriptSalud != null) scriptSalud.CurarAlMaximo();

        // 6. Regresar la animación de la base a Inactiva (sola)
        animatorBase.SetBool("estaCurando", false);

        // 7. Volver a hacer visible a Otix y devolverle el control
        if (spriteOtix != null) spriteOtix.enabled = true;
        if (scriptPlayer != null) scriptPlayer.puedeMoverse = true;

        curando = false;
    }
}