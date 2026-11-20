using UnityEngine;

/// <summary>
/// Controla el patrullaje horizontal automatico de un obstaculo o enemigo, invirtiendo su direccion al detectar el borde de una plataforma o una pared frontal.
/// </summary>
public class ObstaculoMovil : MonoBehaviour
{
    [Header("Configuracion")]
    public float velocidad = 3f;
    public bool empiezaHaciaDerecha = true;

    [Header("Deteccion Automatica")]
    public Transform detectorOrilla; // Punto frontal de referencia desde donde se proyectan los rayos de deteccion de terreno
    public LayerMask capaSuelo;

    private bool movimientoDerecha;

    /// <summary>
    /// Establece la direccion inicial de desplazamiento y orienta la rotacion del objeto segun la configuracion inicial.
    /// </summary>
    void Start()
    {
        movimientoDerecha = empiezaHaciaDerecha;

        if (!movimientoDerecha)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    /// <summary>
    /// Desplaza el objeto hacia adelante y lanza rayos fisicos para verificar si existe suelo continuo o un obstaculo frontal.
    /// </summary>
    void Update()
    {
        // Se utiliza Vector2.right en espacio local para avanzar siempre hacia el frente actual del objeto tras rotarlo
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);

        RaycastHit2D sueloAbajo = Physics2D.Raycast(detectorOrilla.position, Vector2.down, 1f, capaSuelo);

        RaycastHit2D paredFrente = Physics2D.Raycast(detectorOrilla.position, transform.right, 0.2f, capaSuelo);

        if (sueloAbajo.collider == null || paredFrente.collider != null)
        {
            DarVuelta();
        }
    }

    /// <summary>
    /// Invierte el estado de direccion y rota el objeto 180 grados sobre el eje Y para cambiar su sentido de avance.
    /// </summary>
    private void DarVuelta()
    {
        movimientoDerecha = !movimientoDerecha;
        transform.eulerAngles = new Vector3(0, movimientoDerecha ? 0 : 180, 0);
    }
}