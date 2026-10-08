using UnityEngine;

public class DannoTrampa : MonoBehaviour
{


    public int daño = 1; 

    
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
