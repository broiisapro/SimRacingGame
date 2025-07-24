using UnityEngine;
using UnityEngine.Splines;

public class ManualTangentSpline : MonoBehaviour
{
    public SplineContainer splineContainer;
    public GameObject Prometheus;
    public float tangentScale = 0.33f;
    static float fLastHRotation = 0;
    static float fLastVRotation = 0;
    // Sample knot positions
    static Vector3[] positions = new Vector3[]
    {
        new Vector3(0, 0, 0)
    };

    static int iKnots = 20; // Number of knots to generate

    void Start()
    {
        var spline = splineContainer.Spline;
        spline.Clear();

        // Generate a series of knots
        for (int i = 0; i < iKnots; i++)
        {
            AddPosition();
        }

        // Add knots with placeholder tangents
        for (int i = 0; i < iKnots; i++)
        {
            spline.Add(new BezierKnot
            {
                Position = positions[i],
                TangentIn = Vector3.zero,
                TangentOut = Vector3.zero,
                Rotation = Quaternion.identity
            });
        }

        // Manually calculate tangents
        for (int i = 0; i < spline.Count; i++)
        {
            Vector3 prev = (i > 0) ? spline[i - 1].Position : spline[i].Position;
            Vector3 next = (i < spline.Count - 1) ? spline[i + 1].Position : spline[i].Position;

            Vector3 forward = (next - prev) * 0.5f;

            Vector3 tangentIn = -forward * tangentScale;
            Vector3 tangentOut = forward * tangentScale;

            // Update knot (note: Spline is a struct, so use Get/Set)
            BezierKnot knot = spline[i];
            knot.TangentIn = tangentIn;
            knot.TangentOut = tangentOut;
            spline[i] = knot;
        }

        splineContainer.Spline = spline;
    }
    void Update()
    {
        var spline = splineContainer.Spline;
        Vector3 CarPosition = Prometheus.transform.position;

        if ((CarPosition - positions[3]).magnitude <= 100.0f)
        {
            spline.RemoveAt(0); // Remove the first knot
            RemovePosition();
            AddPosition();
            spline.Add(new BezierKnot
            {
                Position = positions[iKnots - 1],
                TangentIn = Vector3.zero,
                TangentOut = Vector3.zero,
                Rotation = Quaternion.identity
            });
            // calculate tangents based on the adjacent points
            Vector3 prev = ((iKnots - 1) > 0) ? spline[(iKnots - 1) - 1].Position : spline[(iKnots - 1)].Position;
            Vector3 next = ((iKnots - 1) < spline.Count - 1) ? spline[(iKnots - 1) + 1].Position : spline[(iKnots - 1)].Position;

            Vector3 forward = (next - prev) * 0.5f;

            Vector3 tangentIn = -forward * tangentScale;
            Vector3 tangentOut = forward * tangentScale;

            // Update knot (note: Spline is a struct, so use Get/Set)
            BezierKnot knot = spline[(iKnots - 1)];
            knot.TangentIn = tangentIn;
            knot.TangentOut = tangentOut;
            spline[(iKnots - 1)] = knot;

            splineContainer.Spline = spline;
        }
    }
    private static float GetHorizontalAngle()
    {
        float fNextRotation = UnityEngine.Random.Range(-15f, 15f);
        fLastHRotation += fNextRotation;
        return fLastHRotation * Mathf.Deg2Rad;
    }
    private static float GetVerticalAngle()
    {
        float fNextRotation = UnityEngine.Random.Range(-5f, 5f);
        fLastVRotation += fNextRotation;
        return fLastVRotation * Mathf.Deg2Rad;
    }
    private static void AddPosition()
    {
        Vector3[] newPositions = new Vector3[positions.Length + 1];
        for (int i = 0; i < positions.Length; i++)
        {
            newPositions[i] = positions[i];
        }
        newPositions[positions.Length] = GenerateKnot(positions[positions.Length - 1]);
        positions = newPositions;
    }
    private static Vector3 GenerateKnot(Vector3 vLastKnotPos)
    {
        float fAngle = GetHorizontalAngle();
        Vector3 vNextKnotPos = new Vector3(100.0f * Mathf.Sin(fAngle), 0, 100.0f * Mathf.Cos(fAngle));
        return vLastKnotPos + vNextKnotPos;
    }
    private static void RemovePosition()
    {
        Vector3[] OldPositions = new Vector3[positions.Length - 1];
        for (int i = 1; i < positions.Length; i++)
        {
            OldPositions[i - 1] = positions[i];
        }
        positions = OldPositions;
    }
}