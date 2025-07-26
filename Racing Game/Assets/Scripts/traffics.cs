using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

public class Traffics : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float speed = 0f;
    public float reachThreshold = 0.5f;

    private float distanceTravelled = 0f;
    private float totalLength;
    private float3 lastPosition;

    void Start()
    {
        
        speed = UnityEngine.Random.Range(0.3f, 0.6f);

        if (splineContainer == null)
        {
            splineContainer = GameObject.FindWithTag("Spline")?.GetComponent<SplineContainer>();
        }

        if (splineContainer == null || splineContainer.Spline == null || splineContainer.Spline.Count < 2)
        {
            Debug.LogError("SplineContainer not found or invalid.");
            enabled = false;
            return;
        }

        totalLength = splineContainer.CalculateLength();
        distanceTravelled = 0f;

        // Initialize position
        splineContainer.Evaluate(distanceTravelled / totalLength, out float3 pos, out float3 tangent, out float3 up);
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(tangent);
        lastPosition = pos;
    }

    void Update()
    {
        if (distanceTravelled >= totalLength)
            return;

        float delta = speed * Time.deltaTime;
        distanceTravelled += delta;

        float t = Mathf.Clamp01(distanceTravelled / totalLength);

        splineContainer.Evaluate(t, out float3 pos, out float3 tangent, out float3 up);

        transform.position = pos;

        if (math.length(tangent) > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(tangent);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.5f * Time.deltaTime);
        }
    }
}
