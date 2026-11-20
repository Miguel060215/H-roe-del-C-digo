using UnityEngine;

/// <summary>
/// Gestiona el dano aplicado al jugador al colisionar con un obstaculo, diferenciando entre impactos frontales y caidas superiores.
/// </summary>
public class DañoObstaculos : MonoBehaviour
{
    public int daño = 1; // Cantidad de puntos de vida que resta el obstaculo al impactar

    /// <summary>
    /// Evalua la direccion del contacto fisico con el jugador para determinar el tipo de penalizacion correspondiente.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SaludJugador saludJugador = collision.gameObject.GetComponent<SaludJugador>();

            if (saludJugador != null)
            {
                bool cayoDesdeArriba = false;

                if (collision.contactCount > 0)
                {
                    Vector2 direccionChoque = collision.GetContact(0).normal;

                    // Un componente Y negativo en la normal de contacto indica que el jugador impacto sobre la superficie superior del obstaculo
                    if (direccionChoque.y < -0.4f)
                    {
                        cayoDesdeArriba = true;
                    }
                }

                if (cayoDesdeArriba)
                {
                    // Aplica el comportamiento de trampa (reposicionamiento y aturdimiento temporal) al aterrizar sobre el obstaculo
                    saludJugador.RecibirDannoTrampa(daño);
                }
                else
                {
                    // Aplica dano directo sin interrumpir el desplazamiento del jugador tras un impacto lateral
                    saludJugador.RecibirDaño(daño);
                }
            }
        }
    }
}