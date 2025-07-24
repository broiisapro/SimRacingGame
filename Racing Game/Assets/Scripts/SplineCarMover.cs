using UnityEngine;
using UnityEngine.Splines;

public class SplineCarMover : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float speed = 5f;

    private float distanceTraveled = 0f;
    private float splineLength;

    void Start()
    {
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer not assigned!");
            enabled = false;
            return;
        }

        splineLength = SplineUtility.CalculateLength(splineContainer.Spline, transform.localToWorldMatrix);

        // Immediately position and orient the car at the start of the spline
        var spline = splineContainer.Spline;
        Vector3 startPosition = spline.EvaluatePosition(0f);
        Vector3 startTangent = spline.EvaluateTangent(0f);
        Vector3 startUp = Vector3.up;

        transform.position = splineContainer.transform.TransformPoint(startPosition);
        transform.rotation = Quaternion.LookRotation(splineContainer.transform.TransformDirection(startTangent), startUp);
    }

    void Update()
    {
        if (splineContainer == null || splineLength <= 0f)
            return;

        distanceTraveled += speed * Time.deltaTime;

        float t = distanceTraveled / splineLength;
        t %= 1f;

        var spline = splineContainer.Spline;
        Vector3 position = spline.EvaluatePosition(t);
        Vector3 tangent = spline.EvaluateTangent(t);
        Vector3 upVector = Vector3.up;

        transform.position = splineContainer.transform.TransformPoint(position);
        transform.rotation = Quaternion.LookRotation(splineContainer.transform.TransformDirection(tangent), upVector);
    }
}