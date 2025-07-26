using UnityEngine;
using UnityEngine.Splines;

public class BigVeinySplineGen : MonoBehaviour
{
    public SplineContainer splineContainer, splineContainer2;

    public int pointCount = 10;
    public float pointSpacing = 750f;
    public float maxZOffset = 160f;

    public Material lineMaterial; // Assign in Inspector

    // Distances from center in order: inner, mid, outer
    private readonly float[] offsets = new float[] { 2.5f, 7.5f, 12.5f };

    void Start()
    {
        Spline centerLine = new Spline();
        Spline[] mirroredSplines = new Spline[6]; // 3 left, 3 right

        for (int i = 0; i < 6; i++)
            mirroredSplines[i] = new Spline();

        Vector3[] centerPositions = new Vector3[pointCount];
        Vector3[][] offsetPositions = new Vector3[6][];
        for (int i = 0; i < 6; i++)
            offsetPositions[i] = new Vector3[pointCount];

        // Step 1: Generate center + mirrored positions
        for (int i = 0; i < pointCount; i++)
        {
            float x = i * pointSpacing;
            float z = Random.Range(-maxZOffset, maxZOffset);
            Vector3 basePos = new Vector3(x, 0, z);
            centerPositions[i] = basePos;

            // Generate positions at offsets from center line
            for (int j = 0; j < offsets.Length; j++)
            {
                offsetPositions[j * 2][i] = basePos + new Vector3(0, 0, -offsets[j]); // Left side
                offsetPositions[j * 2 + 1][i] = basePos + new Vector3(0, 0, offsets[j]); // Right side
            }
        }

        // Step 2: Add knots with tangents
        for (int i = 0; i < pointCount; i++)
        {
            AddKnotToSpline(centerLine, centerPositions, i);

            for (int j = 0; j < 6; j++)
                AddKnotToSpline(mirroredSplines[j], offsetPositions[j], i);
        }

        // Step 3: Add splines to container
        splineContainer2.AddSpline(centerLine);
        foreach (Spline spline in mirroredSplines)
            splineContainer.AddSpline(spline);

        // Step 4: Render all
        RenderSpline(centerLine, "Center", Color.white);
        RenderSpline(mirroredSplines[0], "Inner Left", Color.yellow);
        RenderSpline(mirroredSplines[1], "Inner Right", Color.yellow);
        RenderSpline(mirroredSplines[2], "Mid Left", Color.green);
        RenderSpline(mirroredSplines[3], "Mid Right", Color.green);
        RenderSpline(mirroredSplines[4], "Outer Left", Color.cyan);
        RenderSpline(mirroredSplines[5], "Outer Right", Color.cyan);
    }

    void AddKnotToSpline(Spline spline, Vector3[] positions, int i)
    {
        Vector3 pos = positions[i];
        Vector3 prev = i > 0 ? positions[i - 1] : pos;
        Vector3 next = i < positions.Length - 1 ? positions[i + 1] : pos;

        Vector3 dir = (next - prev).normalized;
        float dist = Vector3.Distance(prev, next) * 0.2f;
        Vector3 tangent = dir * dist;

        spline.Add(new BezierKnot(pos, -tangent, tangent, Quaternion.identity));
    }

    void RenderSpline(Spline spline, string name, Color color)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.parent = this.transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.widthMultiplier = 2f;
        lr.positionCount = 100;
        lr.startColor = lr.endColor = color;
        lr.useWorldSpace = true;

        Vector3[] points = new Vector3[lr.positionCount];
        for (int i = 0; i < points.Length; i++)
        {
            float t = i / (float)(points.Length - 1);
            points[i] = spline.EvaluatePosition(t);
        }

        lr.SetPositions(points);
    }
}
