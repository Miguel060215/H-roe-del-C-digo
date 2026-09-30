using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDCartas : MonoBehaviour
{
    [Header("Configuracion Visual")]
    public Animator[] animadoresCartas;


    public void ActivarIcono(int id)
    {
        if (id >= 0 && id < animadoresCartas.Length)
        {
            // Al encender el Animator, la animación a color comenzará a reproducirse automáticamente
            animadoresCartas[id].enabled = true;
        }
    }
}
