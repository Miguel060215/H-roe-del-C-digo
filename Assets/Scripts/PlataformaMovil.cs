using UnityEngine;

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

    void Start()
    {
        movimientoDerecha = empiezaHaciaDerecha;

        // Los limites se calculan a partir de donde colocas el obstaculo en la escena
        limiteIzquierdo = transform.position.x - rango;
        limiteDerecho = transform.position.x + rango;

        transform.eulerAngles = new Vector3(0, movimientoDerecha ? 0 : 180, 0);
    }

    void Update()
    {
        // Se mueve siempre hacia su "derecha" local (la rotacion en Y define el sentido)
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

    // Evita que se pase del limite por unos cuantos pixeles
    private void AjustarX(float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }

    private void DarVuelta()
    {
        movimientoDerecha = !movimientoDerecha;
        transform.eulerAngles = new Vector3(0, movimientoDerecha ? 0 : 180, 0);
    }

    // Dibuja el rango en la vista Scene para ajustarlo visualmente
    void OnDrawGizmosSelected()
    {
        float centro = Application.isPlaying ? (limiteIzquierdo + limiteDerecho) / 2f : transform.position.x;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(centro - rango, transform.position.y, 0),
                        new Vector3(centro + rango, transform.position.y, 0));
    }
}