using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public Queue<Vector3> Waypoints;
    public int NumWaypoints;
    public float StepDist;
    IEnumerator<Vector3> CurrentFeatureGenerator;

    public PathFeature[] pathFeatures;
    public Color gizmosColor;
    // Start is called before the first frame update
    private void Start()
    {
        Waypoints = new Queue<Vector3>(NumWaypoints);
        CurrentFeatureGenerator = GetFeatureGenerator();
        for (int i = 0; i < NumWaypoints; i++)
        {
            while (!CurrentFeatureGenerator.MoveNext())
            {
                CurrentFeatureGenerator = GetFeatureGenerator();
            }
            Waypoints.Enqueue(CurrentFeatureGenerator.Current);
        }
        Gizmos.color = gizmosColor;
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
    // Update is called once per frame
    void Update()
    {

    }
}
