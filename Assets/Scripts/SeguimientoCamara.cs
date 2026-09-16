using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
   public Transform target; // El objetivo que la cámara seguirá
    private void LateUpdate() {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
