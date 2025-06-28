using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class SplineHighwayGenerator : MonoBehaviour
{
    [Header("Spline Settings")]
    public int numPoints = 10000;
    public float segmentLength = 1f;
    public float maxTurnAngle = 25f; // in degrees

    [Header("Spline Output")]
    public List<Vector3> splinePoints = new List<Vector3>();

    private void OnValidate()
    {
        GenerateSpline();
    }

    void GenerateSpline()
    {
        splinePoints.Clear();
        Vector3 currentPos = transform.position;
        Vector3 currentDir = transform.forward;

        splinePoints.Add(currentPos);

        for (int i = 0; i < numPoints; i++)
        {
            // Randomly turn left or right
            float angle = Random.Range(-maxTurnAngle, maxTurnAngle);
            currentDir = Quaternion.Euler(0f, angle, 0f) * currentDir;
            currentPos += currentDir.normalized * segmentLength;
            splinePoints.Add(currentPos);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < splinePoints.Count - 1; i++)
        {
            Gizmos.DrawLine(splinePoints[i], splinePoints[i + 1]);
        }
    }
}
