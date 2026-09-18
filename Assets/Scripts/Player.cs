using UnityEngine;

public class Player : MonoBehaviour
{
    //variables
    public float speed = 4;
    private Rigidbody2D rb2D;
    private float move;
    public float jumpForce = 7;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    // Variables para la escalera
    [Header("Climbing")]
    public float climbSpeed = 5f;
    private bool isClimbing;
    private float verticalInput;
    private float initialGravity;
    private Animator animator;

    void Start()
    {
        //Aqui dentro se ejecutara el codigo al iniciar el juego
        rb2D = GetComponent<Rigidbody2D>();
        initialGravity = rb2D.gravityScale; // Guardar la gravedad inicial
        animator = GetComponent<Animator>();
    }


    void Update()
    {
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
        }

        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("SpeedY", rb2D.linearVelocityY);
        animator.SetBool("enSuelo", isGrounded);
    }

    private void FixedUpdate()
    {
        //Aqui dentro se ejecutara el codigo cada vez que se actualice la fisica del juego
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (isClimbing)
        {
            rb2D.gravityScale = 0f;
        }
        else
        {
            rb2D.gravityScale = initialGravity;
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
}