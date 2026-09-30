using UnityEngine;

public class ObstaculoMovil : MonoBehaviour
{
    [Header("Configuracion")]
    public float velocidad = 3f;
    public bool empiezaHaciaDerecha = true;

    [Header("Deteccion Automatica")]
    public Transform detectorOrilla;
    public LayerMask capaSuelo;

    private bool movimientoDerecha;

    void Start()
    {
        movimientoDerecha = empiezaHaciaDerecha;

        if (!movimientoDerecha) {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(Vector2.right * velocidad * Time.deltaTime);

        RaycastHit2D sueloAbajo = Physics2D.Raycast(detectorOrilla.position, Vector2.down, 1f, capaSuelo);

        RaycastHit2D paredFrente = Physics2D.Raycast(detectorOrilla.position, transform.right, 0.2f, capaSuelo);

        if (sueloAbajo.collider == null || paredFrente.collider != null)
        {
            DarVuelta();
        }
    }



    private void DarVuelta() {
        movimientoDerecha = !movimientoDerecha;
        transform.eulerAngles = new Vector3(0, movimientoDerecha ? 0 : 180, 0);
    }
}
