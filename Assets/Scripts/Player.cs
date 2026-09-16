using UnityEngine;

public class Player : MonoBehaviour
{
    //variables
    public float speed = 5;
    private Rigidbody2D rb2D;
    private float move;
    public float jumpForce = 7;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;



    void Start()
    {
        //Aqui dentro se ejecutara el codigo al iniciar el juego
        rb2D = GetComponent<Rigidbody2D>();

    }


    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");//Uniti tiene definido la palabra Horizontal para el movimiento horizontal "a","d" y "<",">"
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move) * Mathf.Abs(transform.localScale.x), 3, 3);
            //Nota para mi: como el personaje lo tengi escalado a 4, multiplico por 4 para que no se vea chiquito en el eje x
        }
        if (Input.GetButtonDown("Jump")&& isGrounded) {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            
        }

    }

    private void FixedUpdate()
    {
        //Aqui dentro se ejecutara el codigo cada vez que se actualice la fisica del juego
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }
}