using UnityEngine;
using System.Collections;

/// <summary>
/// Controla el seguimiento suavizado de la camara hacia el jugador y gestiona las transiciones de acercamiento ortografico (zoom) en eventos especificos.
/// </summary>
public class SeguimientoCamara : MonoBehaviour
{
    public Transform target; // Objetivo al que seguira la camara
    public float suavizado = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Desplazamiento relativo respecto al objetivo para mantener la profundidad Z de la camara

    private Camera cam;
    private float tamanoOriginal;

    /// <summary>
    /// Obtiene la referencia al componente Camera y almacena el tamano ortografico inicial de la escena.
    /// </summary>
    void Start()
    {
        cam = GetComponent<Camera>();
        tamanoOriginal = cam.orthographicSize;
    }

    /// <summary>
    /// Actualiza la posicion de la camara mediante interpolacion lineal despues de que el objetivo haya completado su movimiento en el fotograma.
    /// </summary>
    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 posicionDeseada = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
        }
    }

    /// <summary>
    /// Inicia la corrutina encargada de modificar gradualmente el acercamiento de la camara.
    /// </summary>
    public void IniciarZoom(float tamanoFinal, float duracion)
    {
        StartCoroutine(RutinaZoom(tamanoFinal, duracion));
    }

    /// <summary>
    /// Interpola el tamano ortografico de la camara desde su valor actual hasta el objetivo durante el tiempo especificado.
    /// </summary>
    private IEnumerator RutinaZoom(float tamanoFinal, float duracion)
    {
        float tiempo = 0;
        float tamanoInicial = cam.orthographicSize;

        while (tiempo < duracion)
        {
            cam.orthographicSize = Mathf.Lerp(tamanoInicial, tamanoFinal, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }
        cam.orthographicSize = tamanoFinal;
    }
}