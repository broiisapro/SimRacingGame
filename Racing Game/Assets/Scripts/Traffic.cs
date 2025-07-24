using UnityEngine;
using System.Collections.Generic;

public class Traffic : MonoBehaviour
{
    public SplineHighwayGenerator spline;
    public float speed;
    public float reachThreshold = 0.5f;
    private int currentLane;
    private int currentIndex = 0;
    private List<Vector3> path;

    void Start()
    {
        speed = Random.Range(5f, 15f);

        if (spline == null)
        {
            spline = GameObject.FindWithTag("Spline")?.GetComponent<SplineHighwayGenerator>();
        }

        if (spline == null || spline.lanePoints.Count == 0)
        {
            Debug.LogError("Spline not found or empty.");
            enabled = false;
            return;
        }

        // Randomly assign a lane to this vehicle
        currentLane = Random.Range(0, spline.numberOfLanes);
        path = spline.lanePoints[currentLane];

        // Position the vehicle at the start of its assigned lane
        transform.position = path[0];
        currentIndex = 1;
    }

    void Update()
    {
        if (currentIndex >= path.Count)
            return;

        Vector3 target = path[currentIndex];
        Vector3 direction = (target - transform.position).normalized;
        float step = speed * Time.deltaTime;

        // Move toward target point
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        // Face the direction of movement
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }

        // If close to target, go to next point
        if (Vector3.Distance(transform.position, target) < reachThreshold)
        {
            currentIndex++;
        }
    }
}
