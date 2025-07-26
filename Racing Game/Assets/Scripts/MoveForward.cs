using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 10f;
    public Transform player;      // Reference to Prometheus
    public float despawnDistance = 30f; // How far behind before despawning

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
