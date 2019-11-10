using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private Queue<Vector3> waypoints;
    public int NumWaypoints;
    public float StepDist;
    IEnumerator<Vector3> CurrentFeatureGenerator;
    public PathFeature[] pathFeatures;
    public Color gizmosColor;

    public Vector3[] TestWaypoints = new Vector3[]
    {
        Vector3.zero,
        5 * Vector3.forward
    };
    public Queue<Vector3> Waypoints
    {
        get
        {
            return waypoints ?? (waypoints = new Queue<Vector3>(TestWaypoints)); //Default to TestWaypoints (for in Editor, when Start() hasn't been called but gizmos still need to happen)
        }
        set => waypoints = value;
    }

    private Transform player;

    // Start is called before the first frame update
    private void Start()
    {
        Waypoints = new Queue<Vector3>(NumWaypoints);
        Waypoints.Enqueue(Vector3.zero); //Our first act of business is to add the starting point.
        SendMessage("WaypointAdded");
        CurrentFeatureGenerator = GetFeatureGenerator();
        for (int i = 0; i < NumWaypoints-1; i++)
        {
            AddNewPoint();
        }
        Gizmos.color = gizmosColor;
        player = GameObject.FindWithTag("Player").transform;
    }

    private void AddNewPoint()
    {
        while (!CurrentFeatureGenerator.MoveNext())
        {
            CurrentFeatureGenerator = GetFeatureGenerator();
        }
        Waypoints.Enqueue(CurrentFeatureGenerator.Current);
        SendMessage("WaypointAdded");
    }
    private void RemoveOldPoint()
    {
        Waypoints.Dequeue(); 
        SendMessage("WaypointRemoved");
    }

    IEnumerator<Vector3> GetFeatureGenerator()
    {
        Vector3 startPosition = Waypoints?.LastOrDefault() ?? Vector3.zero;
        Vector3 startDirection = Vector3.forward;
        if (Waypoints?.Count >= 2)
        {
            startDirection = startPosition - Waypoints.ElementAt(Waypoints.Count - 2); //1 from end
            startDirection.Normalize();
        }
        return pathFeatures.Single().GetGenerator(startPosition, startDirection, StepDist, 50f);
    }
    private void Update()
    {
        if (NumWaypoints < 3)
        {
            Debug.LogWarning("Waypoint Path Following doesn't work with fewer than three waypoints");
            return;
        }
        Vector3 playerPos = player.position;
        float d2 = (Waypoints.ElementAt(1) - playerPos).magnitude; //Distance from player to point 2 (index 1)
        float d3 = (Waypoints.ElementAt(2) - playerPos).magnitude; //Distance from player to point 3 (index 2)
        if (d3 < d2) //Basically, when we're over halfway into the second segment (between points 2 and 3) ...
        {
            RemoveOldPoint(); // ...Take off the now-unseen segment
            AddNewPoint();    // ... and add a new one in the distance.
        }
    }

    private void OnDrawGizmos()
    {
        if (Waypoints == null) return;
        var waypointArr = Waypoints.ToArray();
        for (int i = 0; i < Waypoints.Count; i++)
        {
            Gizmos.DrawWireSphere(waypointArr[i], StepDist / 4);
            if (i == 0)
            {
                continue;
            }
            Gizmos.DrawLine(waypointArr[i - 1], waypointArr[i]);
        }
    }
}
