using UnityEngine;
using System.Collections;

//Este script de encargaa de hacer que la camara siga al jugador, y ademas que al morir haga un zoom de acercamiento a donde esta el jugador :)
public class SeguimientoCamara : MonoBehaviour
{
    public Transform target; // El objetivo que la cámara seguirá
    public float suavizado = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private Camera cam;
    private float tamanoOriginal;

    void Start()
    {
        cam = GetComponent<Camera>();
        tamanoOriginal = cam.orthographicSize;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 posicionDeseada = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
        }
    }

    public void IniciarZoom(float tamanoFinal, float duracion)
    {
        StartCoroutine(RutinaZoom(tamanoFinal, duracion));
    }

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