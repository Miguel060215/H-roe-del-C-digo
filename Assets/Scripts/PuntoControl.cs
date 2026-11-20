using System.Collections.Specialized;
using UnityEngine;

/// <summary>
/// Registra la posicion de un punto de control (checkpoint) como la nueva coordenada global de reaparicion cuando el jugador cruza su area de activacion.
/// </summary>
public class PuntoControl : MonoBehaviour
{
    /// <summary>
    /// Detecta el contacto con el jugador y actualiza las variables estaticas de reaparicion en la clase Player.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.puntoReaparicion = transform.position;
            Player.hayPuntoGuardado = true;

            Debug.Log("Punto de reaparición guardado.");
        }
    }
}