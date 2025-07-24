using UnityEngine;

public class traffic : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward / 200);
    }
}