using UnityEngine;

/// <summary>
/// Aplica dano al jugador cuando entra en contacto con el area de colision de una trampa.
/// </summary>
public class DannoTrampa : MonoBehaviour
{
    public int daño = 1; // Cantidad de puntos de vida que resta la trampa al activarse

    /// <summary>
    /// Detecta el ingreso de un colisionador y transfiere el dano si corresponde al jugador.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SaludJugador saludJugador = other.GetComponent<SaludJugador>();

            if (saludJugador != null)
            {
                saludJugador.RecibirDannoTrampa(daño);
            }
        }
    }
}