using UnityEngine;

public class DañoObstaculos : MonoBehaviour
{
    public int daño = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SaludJugador saludJugador = collision.gameObject.GetComponent<SaludJugador>();

            if (saludJugador != null)
            {
                bool cayoDesdeArriba = false;

                // revisamos de qeu lado pego el jugador para saber si cayo ensima o choco de lado
                if (collision.contactCount > 0)
                {
                    Vector2 direccionChoque = collision.GetContact(0).normal;

                    // el vector normal apunta de abajo hacia arriba cuando le caemos ensima (negativo)
                    if (direccionChoque.y < -0.4f)
                    {
                        cayoDesdeArriba = true;
                    }
                }

                if (cayoDesdeArriba)
                {
                    // le callo ensima, lo regresamos a tierra firme con un segundo de stun
                    saludJugador.RecibirDannoTrampa(daño);
                }
                else
                {
                    // choco de frente asi que solo quitamos una bateria pero sigue corriendo normal
                    saludJugador.RecibirDaño(daño);
                }
            }
        }
    }
}