using UnityEngine;

/// <summary>
/// Controla el desplazamiento horizontal continuo de una plataforma entre dos limites calculados a partir de su posicion inicial y un rango configurado.
/// </summary>
public class PlataformaMovil : MonoBehaviour
{
    [Header("Configuracion")]
    public float velocidad = 3f;
    public bool empiezaHaciaDerecha = true;

    [Header("Rango de movimiento")]
    [Tooltip("Distancia (en unidades) que recorre hacia cada lado desde su posicion inicial")]
    public float rango = 5f;

    private bool movimientoDerecha;
    private float limiteIzquierdo;
    private float limiteDerecho;

    /// <summary>
    /// Calcula las coordenadas limite de desplazamiento respecto a la posicion inicial y orienta la plataforma segun su direccion de partida.
    /// </summary>
    void Start()
    {
        movimientoDerecha = empiezaHaciaDerecha;

        limiteIzquierdo = transform.position.x - rango;
        limiteDerecho = transform.position.x + rango;

        transform.eulerAngles = new Vector3(0, movimientoDerecha ? 0 : 180, 0);
    }

    /// <summary>
    /// Desplaza la plataforma horizontalmente y verifica si alcanza alguno de los limites establecidos para corregir su posicion e invertir su sentido.
    /// </summary>
    void Update()
    {
        // Se utiliza Vector2.right en espacio local; la rotacion de 180 grados en el eje Y define si avanza hacia la izquierda o derecha en el mundo
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);

        float x = transform.position.x;

        if (movimientoDerecha && x >= limiteDerecho)
        {
            AjustarX(limiteDerecho);
            DarVuelta();
        }
        else if (!movimientoDerecha && x <= limiteIzquierdo)
        {
            AjustarX(limiteIzquierdo);
            DarVuelta();
        }
    }

    /// <summary>
    /// Fija exactamente la coordenada X de la plataforma para evitar desfases acumulados por velocidad al alcanzar un extremo.
    /// </summary>
    private void AjustarX(float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }

    /// <summary>
    /// Invierte la bandera de direccion y rota el objeto 180 grados sobre el eje Y.
    /// </summary>
    private void DarVuelta()
    {
        movimientoDerecha = !movimientoDerecha;
        transform.eulerAngles = new Vector3(0, movimientoDerecha ? 0 : 180, 0);
    }

    /// <summary>
    /// Dibuja una linea guia amarilla en la ventana Scene al seleccionar el objeto para visualizar el recorrido total de la plataforma.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Evalua Application.isPlaying para mantener la guia centrada en el punto de origen real incluso cuando la plataforma ya esta en movimiento
        float centro = Application.isPlaying ? (limiteIzquierdo + limiteDerecho) / 2f : transform.position.x;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(centro - rango, transform.position.y, 0),
                        new Vector3(centro + rango, transform.position.y, 0));
    }
}