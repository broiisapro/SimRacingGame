using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class SplineHighwayGenerator : MonoBehaviour
{
    [Header("Spline Settings")]
    public int numPoints = 50;
    public float segmentLength = 2f;
    public float maxTurnAngle = 10f;
    public float roadWidth = 8f;
    public int smoothingResolution = 100;

    [Header("Road Visuals")]
    public Material roadMaterial;

    [HideInInspector]
    public List<Vector3> splinePoints = new List<Vector3>();

    public int numberOfLanes = 4;
    private float laneWidth;

    [HideInInspector]
    public List<List<Vector3>> lanePoints = new List<List<Vector3>>();

    private Mesh roadMesh;

    void Start()
    {
        laneWidth = roadWidth / numberOfLanes;
        GenerateSpline();
        GenerateRoadMesh();
        GenerateLanePoints();
    }

    public void GenerateSpline()
    {
        splinePoints.Clear();
        Vector3 currentPos = transform.position;
        Vector3 currentDir = transform.forward;

        splinePoints.Add(currentPos);

        for (int i = 0; i < numPoints; i++)
        {
            float angle = Random.Range(-maxTurnAngle, maxTurnAngle);
            currentDir = Quaternion.Euler(0f, angle, 0f) * currentDir;
            currentPos += currentDir.normalized * segmentLength;
            splinePoints.Add(currentPos);
        }
    }

    List<Vector3> GetSmoothedPoints()
    {
        List<Vector3> smoothPoints = new List<Vector3>();

        for (int i = 0; i < splinePoints.Count - 3; i++)
        {
            Vector3 p0 = splinePoints[i];
            Vector3 p1 = splinePoints[i + 1];
            Vector3 p2 = splinePoints[i + 2];
            Vector3 p3 = splinePoints[i + 3];

            for (int j = 0; j < smoothingResolution; j++)
            {
                float t = j / (float)smoothingResolution;
                Vector3 point = CatmullRom(p0, p1, p2, p3, t);
                smoothPoints.Add(point);
            }
        }

        return smoothPoints;
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            2 * p1 +
            (-p0 + p2) * t +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
            (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t
        );
    }

    void GenerateRoadMesh()
    {
        var points = GetSmoothedPoints();
        if (points.Count < 2) return;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];
            Vector3 direction = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, direction) * (roadWidth * 0.5f);

            Vector3 leftStart = start - right;
            Vector3 rightStart = start + right;
            Vector3 leftEnd = end - right;
            Vector3 rightEnd = end + right;

            int baseIndex = vertices.Count;
            vertices.Add(leftStart);
            vertices.Add(rightStart);
            vertices.Add(leftEnd);
            vertices.Add(rightEnd);

            triangles.Add(baseIndex + 0);
            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 3);
        }

        if (roadMesh == null) roadMesh = new Mesh();
        roadMesh.Clear();
        roadMesh.SetVertices(vertices);
        roadMesh.SetTriangles(triangles, 0);
        roadMesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = roadMesh;
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = roadMesh;

        if (roadMaterial != null)
        {
            GetComponent<MeshRenderer>().sharedMaterial = roadMaterial;
        }
    }

    private void GenerateLanePoints()
    {
        var smoothedPoints = GetSmoothedPoints();
        lanePoints.Clear();

        // Initialize lists for each lane
        for (int lane = 0; lane < numberOfLanes; lane++)
        {
            lanePoints.Add(new List<Vector3>());
        }

        // Calculate points for each lane
        for (int i = 0; i < smoothedPoints.Count - 1; i++)
        {
            Vector3 start = smoothedPoints[i];
            Vector3 end = smoothedPoints[i + 1];
            Vector3 direction = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, direction);

            // Calculate lane offsets from the center
            for (int lane = 0; lane < numberOfLanes; lane++)
            {
                float laneOffset = (lane - (numberOfLanes - 1) / 2.0f) * laneWidth;
                Vector3 lanePoint = start + right * laneOffset;
                lanePoints[lane].Add(lanePoint);
            }
        }
    }
}
