using System.Collections.Specialized;
using UnityEngine;

public class PuntoControl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { 
            Player.puntoReaparicion = transform.position;
            Player.hayPuntoGuardado = true;

            Debug.Log("Punto de reaparición guardado.");
        }
    }
}
