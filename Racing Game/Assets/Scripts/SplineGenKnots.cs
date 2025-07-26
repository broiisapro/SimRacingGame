using UnityEngine;
using UnityEngine.Splines;

public class StraightRoadSpline : MonoBehaviour
{
    public SplineContainer splineContainer;
    public GameObject Prometheus;
    public int knotCount = 20;
    public float segmentLength = 50f;
    public GameObject carPrefab;
    public int[] Roff = new int[4];
    public GameObject wallPrefab;



    private Vector3[] positions;

    void Start()
    {
        Roff[0] = 2;
        Roff[1] = -2;
        Roff[2] = 6;
        Roff[3] = -6;
        positions = new Vector3[knotCount];

        for (int i = 0; i < knotCount; i++)
        {
            positions[i] = new Vector3(0, 0, i * segmentLength);
        }

        CreateLinearSpline();
    }

    void Update()
    {
        Vector3 carPos = Prometheus.transform.position;

        if ((carPos - positions[3]).magnitude <= 50f)
        {
            ShiftPositionsForward();
            CreateLinearSpline();
        }
    }

    void ShiftPositionsForward()
    {
        for (int i = 0; i < knotCount - 1; i++)
        {
            positions[i] = positions[i + 1];
            
        }

        positions[knotCount - 1] = positions[knotCount - 2] + new Vector3(0, 0, segmentLength);
    }

    void CreateLinearSpline()
    {
        var spline = splineContainer.Spline;
        spline.Clear();

        for (int i = 0; i < positions.Length; i++)
        {
            int rInt = Random.Range(0, 4);
            var knot = new BezierKnot(positions[i], Vector3.zero, Vector3.zero, Quaternion.identity);
            spline.Add(knot);
            if(i == positions.Length - 1) 
            {
                Vector3 spawnpPos = positions[i];
                spawnpPos.x = spawnpPos.x + Roff[rInt];
                Instantiate(carPrefab, spawnpPos, Quaternion.identity);
            }



#if UNITY_2023_1_OR_NEWER
            spline.SetTangentMode(i, TangentMode.Linear);
#endif
        }
    }
}
