using UnityEngine;
using UnityEngine.Splines;

public class FollowerFor : MonoBehaviour
{
    private SplineContainer splineContainer;
    private float speed = 5f;
    private float t = 0f;

    public void Init(SplineContainer container, float moveSpeed)
    {
        splineContainer = container;
        speed = moveSpeed;
    }

    void Update()
    {
        if (splineContainer == null) return;

        t += (speed / splineContainer.CalculateLength()) * Time.deltaTime;
        t = Mathf.Clamp01(t);

        Vector3 pos = splineContainer.Spline.EvaluatePosition(t);
        transform.position = pos;

        if (t >= 1f)
            Destroy(gameObject);
    }
}