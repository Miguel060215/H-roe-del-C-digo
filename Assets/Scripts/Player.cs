using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    //variables
    public float speed = 5;
    private Rigidbody2D rb2D;
    private float move;
    public float jumpForce = 7;
    public bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    [HideInInspector] public Vector3 ultimaPosicionSegura; // Variable para almacenar la última posición segura del jugador


    //variables para deteccion de paredes
    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallRadius = 0.2f;
    private bool isTouchingWall;

    // Variables para la escalera
    [Header("Climbing")]
    public float climbSpeed = 5f;
    private bool isClimbing;
    private float verticalInput;
    private float initialGravity;
    private Animator animator;

    [Header("Control de Estado")]
    public bool puedeMoverse = true; // Variable para controlar si el jugador puede moverse

    [Header("Audio")]
    public AudioSource fuenteCaminar;
    public AudioSource fuenteEfectos;
    public AudioClip clipSalto;
    [Header("Sistema de Reaparición")]
    public static Vector3 puntoReaparicion; // Variable estática para almacenar el punto de reaparición
    public static bool hayPuntoGuardado = false;

    void Start()
    {
        //Aqui dentro se ejecutara el codigo al iniciar el juego
       // ultimaPosicionSegura = transform.position; // Inicializar la última posición segura al inicio del juego
        rb2D = GetComponent<Rigidbody2D>();
        initialGravity = rb2D.gravityScale; // Guardar la gravedad inicial
        animator = GetComponent<Animator>();
        if (hayPuntoGuardado) { 
            transform.position = puntoReaparicion;
        }
    }


    void Update()
    {
        if (!puedeMoverse) {
            rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
            animator.SetFloat("Speed", 0);
            return; 
        }
        move = Input.GetAxisRaw("Horizontal");//Uniti tiene definido la palabra Horizontal para el movimiento horizontal "a","d" y "<",">"

        // Capturar entrada vertical para la escalera
        verticalInput = Input.GetAxisRaw("Vertical");

        if (!isClimbing)
        {
            rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);
        }
        else
        {
            // Movimiento libre en X y velocidad vertical controlada en la escalera
            rb2D.linearVelocity = new Vector2(move * speed, verticalInput * climbSpeed);
        }

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move) * Mathf.Abs(transform.localScale.x), 4, 4);
            //Nota para mi: como el personaje lo tengi escalado a 4, multiplico por 4 para que no se vea chiquito en el eje x
        }
        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            fuenteEfectos.PlayOneShot(clipSalto,0.8f);
        }

        bool pushing = isGrounded && isTouchingWall && (move != 0);

        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("SpeedY", rb2D.linearVelocity.y);
        animator.SetBool("enSuelo", isGrounded);
        animator.SetBool("empuje", pushing);

        bool estaCaminando = Mathf.Abs(move) > 0 && isGrounded && !isClimbing;
        if (estaCaminando && !fuenteCaminar.isPlaying)
        {
            fuenteCaminar.Play();
        }
        else if (!estaCaminando && fuenteCaminar.isPlaying) { 
            fuenteCaminar.Stop();
        }
    }

    private void FixedUpdate()
    {
        //Aqui dentro se ejecutara el codigo cada vez que se actualice la fisica del juego
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (isGrounded) { 
            ultimaPosicionSegura = transform.position; // actualiza la uultima posición segura cuando el jugador este en el suelo
        }

        if (wallCheck != null) {
            isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallRadius, groundLayer);
        }

       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
        }
    }

    public bool EstaCallendo() {
        return rb2D.linearVelocity.y < -0.1f && !isGrounded;
    }

    public void BloquarControles(float tiempo) {
        StartCoroutine(RutinaBloqueo(tiempo));
    }

    private IEnumerator RutinaBloqueo(float tiempo) {
        puedeMoverse = false;
        yield return new WaitForSeconds(tiempo);
        puedeMoverse = true;
    }
}