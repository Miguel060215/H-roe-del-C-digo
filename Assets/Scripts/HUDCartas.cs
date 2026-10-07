using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCartas : MonoBehaviour
{
    [Header("Configuracion Visual")]
    public Animator[] animadoresCartas;

    public static List<int> cartasRecolectadas = new List<int>();

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