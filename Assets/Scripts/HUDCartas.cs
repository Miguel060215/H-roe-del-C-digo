using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Administra los indicadores visuales de las cartas recolectadas en el HUD y conserva el registro global de identificadores obtenidos entre escenas.
/// </summary>
public class HUDCartas : MonoBehaviour
{
    [Header("Configuracion Visual")]
    public Animator[] animadoresCartas;

    public static List<int> cartasRecolectadas = new List<int>(); // Lista estatica que persiste los IDs de las cartas obtenidas para sincronizar el HUD al recargar niveles

    /// <summary>
    /// Restaura el estado visual de los iconos en el HUD recorriendo las cartas registradas previamente en la lista estatica.
    /// </summary>
    void Start()
    {
        foreach (int id in cartasRecolectadas)
        {
            if (id >= 0 && id < animadoresCartas.Length)
            {
                animadoresCartas[id].enabled = true;
            }
        }
    }

    /// <summary>
    /// Habilita el componente Animator correspondiente al identificador recibido y registra la carta en la lista global si aun no existe.
    /// </summary>
    public void ActivarIcono(int id)
    {
        if (id >= 0 && id < animadoresCartas.Length)
        {
            animadoresCartas[id].enabled = true;

            if (!cartasRecolectadas.Contains(id))
            {
                cartasRecolectadas.Add(id);
            }
        }
    }
}