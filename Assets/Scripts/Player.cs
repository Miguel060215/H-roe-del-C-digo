using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla el movimiento horizontal, salto, doble salto, escalada, animaciones, efectos de sonido y puntos de reaparicion del jugador.
/// </summary>
public class Player : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rb2D;
    private float move;
    public float jumpForce = 7;
    public bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    [HideInInspector] public Vector3 ultimaPosicionSegura; // Registra la ultima coordenada estable en tierra firme para reubicar al jugador tras caer en una trampa

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallRadius = 0.2f;
    private bool isTouchingWall;

    [Header("Climbing")]
    public float climbSpeed = 5f;
    private bool isClimbing;
    private float verticalInput;
    private float initialGravity; // Almacena la escala de gravedad original para restaurarla al salir de una escalera
    private Animator animator;

    [Header("Control de Estado")]
    public bool puedeMoverse = true;

    [Header("Audio")]
    public AudioSource fuenteCaminar;
    public AudioSource fuenteEfectos;
    public AudioClip clipSalto;

    [Header("Sistema de Reaparición")]
    public static Vector3 puntoReaparicion; // Coordenada global persistente donde reaparece el jugador tras morir
    public static bool hayPuntoGuardado = false;

    [Header("Doble Salto")]
    public static bool tieneDobleSalto = false; // Bandera estatica que indica si el jugador ya desbloqueo la habilidad en la partida
    public static string escenaDobleSalto = ""; // Registra el nombre de la escena donde se obtuvo la habilidad para reiniciarla al avanzar de nivel
    private bool puedeDobleSalto = false;

    /// <summary>
    /// Inicializa las referencias de fisica y animacion, guarda la gravedad base y traslada al jugador al punto de control si existe uno activo.
    /// </summary>
    void Start()
    {
        if (escenaDobleSalto != SceneManager.GetActiveScene().name)
        {
            tieneDobleSalto = false;
        }

        // ultimaPosicionSegura = transform.position;
        rb2D = GetComponent<Rigidbody2D>();
        initialGravity = rb2D.gravityScale;
        animator = GetComponent<Animator>();
        if (hayPuntoGuardado)
        {
            transform.position = puntoReaparicion;
        }
    }

    /// <summary>
    /// Procesa las entradas del usuario, actualiza velocidades de desplazamiento y salto, sincroniza el audio de pasos y actualiza los parametros del Animator.
    /// </summary>
    void Update()
    {
        if (!puedeMoverse)
        {
            rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
            animator.SetFloat("Speed", 0);
            animator.SetFloat("SpeedY", 0);
            animator.SetBool("enSuelo", true);
            return;
        }

        move = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (!isClimbing)
        {
            rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);
        }
        else
        {
            rb2D.linearVelocity = new Vector2(move * speed, verticalInput * climbSpeed);
        }

        if (move != 0)
        {
            // Invierte el signo en el eje X manteniendo la magnitud base de escala (4) para orientar el sprite segun la direccion de avance
            transform.localScale = new Vector3(Mathf.Sign(move) * Mathf.Abs(transform.localScale.x), 4, 4);
        }

        if (isGrounded || isClimbing)
        {
            puedeDobleSalto = true;
        }

        if (Input.GetButtonDown("Jump") && !isClimbing)
        {
            if (isGrounded)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
                fuenteEfectos.PlayOneShot(clipSalto, 0.8f);
            }
            else if (!isGrounded && puedeDobleSalto && tieneDobleSalto)
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
                fuenteEfectos.PlayOneShot(clipSalto, 0.8f);
                puedeDobleSalto = false; // Consume el salto adicional hasta que el jugador vuelva a tocar el suelo o una escalera
            }
        }

        bool pushing = isGrounded && isTouchingWall && (move != 0);
        bool estaCaminando = Mathf.Abs(move) > 0 && isGrounded && !isClimbing;
        if (estaCaminando && !fuenteCaminar.isPlaying)
        {
            fuenteCaminar.Play();
        }
        else if (!estaCaminando && fuenteCaminar.isPlaying)
        {
            fuenteCaminar.Stop();
        }

        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("SpeedY", rb2D.linearVelocity.y);
        animator.SetBool("enSuelo", isGrounded);
        animator.SetBool("empuje", pushing);
    }

    /// <summary>
    /// Evalua mediante superposicion circular la deteccion de suelo y paredes en intervalos fijos de fisica, actualizando la ultima posicion segura.
    /// </summary>
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (isGrounded)
        {
            ultimaPosicionSegura = transform.position;
        }

        if (wallCheck != null)
        {
            isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallRadius, groundLayer);
        }
    }

    /// <summary>
    /// Habilita el modo de escalada y anula la gravedad al entrar en contacto con una escalera.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb2D.gravityScale = 0f;
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0);
        }
    }

    /// <summary>
    /// Desactiva el modo de escalada y restaura la gravedad original al abandonar el area de la escalera.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb2D.gravityScale = initialGravity;
        }
    }

    /// <summary>
    /// Determina si el personaje se encuentra descendiendo en el aire sin contacto con el suelo.
    /// </summary>
    public bool EstaCallendo()
    {
        return rb2D.linearVelocity.y < -0.1f && !isGrounded;
    }

    /// <summary>
    /// Inicia una corrutina que suspende temporalmente el control de movimiento del jugador.
    /// </summary>
    public void BloquarControles(float tiempo)
    {
        StartCoroutine(RutinaBloqueo(tiempo));
    }

    /// <summary>
    /// Deshabilita el movimiento durante los segundos indicados y lo reactiva unicamente si el jugador sigue con vida.
    /// </summary>
    private IEnumerator RutinaBloqueo(float tiempo)
    {
        puedeMoverse = false;
        yield return new WaitForSeconds(tiempo);

        SaludJugador salud = GetComponent<SaludJugador>();
        if (salud != null && !salud.estaMuerto)
        {
            puedeMoverse = true;
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Debug.Log("<color=red>OTIX CHOCÓ CONTRA:</color> " + collision.gameObject.name);
    }*/
}