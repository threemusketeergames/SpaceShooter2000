using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class PathTubeSpawner : MonoBehaviour
{
    public PathManager pathManager;

    public GameObject TubePrefab;
    public List<GameObject> TubeObjects;

    public float innerRadius = 6f;
    public float outerRadius = 10f;
    public float density = 0.1f;
    public int numTubeVertices = 40;

    public bool enableTubeGizmo = true;

    public int numCornerBands = 10;
    public int numTubeLines = 8;

    public GameObject[] asteroidModels;

    public Vector3[] TestWaypoints = new Vector3[]
    {
        Vector3.zero,
        5 * Vector3.forward
    };

    private Vector3[] firstCircle;
    // Use this for initialization
    void Start()
    {
        for (int i = 1; i < pathManager.Waypoints.Count; i++)
        {
            SpawnForWaypoint(i);
        }
    }

    void SpawnForWaypoint(int index)
    {
        if (index > 0)
        {
            Vector3? firstPoint;
            if (index > 1)
            {
                firstPoint = pathManager.Waypoints.ElementAt(index - 2);
            }
            else
            {
                firstPoint = null;
            }

            Vector3 centerPoint = pathManager.Waypoints.ElementAt(index - 1);
            Vector3 newPoint = pathManager.Waypoints.ElementAt(index);
            float outerSphereVolume = 4 / 3 * Mathf.PI * Mathf.Pow(outerRadius, 3);

            SegmentStuffs segmentStuffs = new SegmentStuffs(centerPoint, newPoint);
            float segmentLength = segmentStuffs.segment.magnitude;
            float hollowedCircleArea = 2 * Mathf.PI * (Mathf.Pow(outerRadius, 2) - Mathf.Pow(innerRadius, 2)); // Subtract big circle by little circle.  I factored out the 2pi like a total math pro.
            float spawnVolume = hollowedCircleArea * segmentLength;
            int spawnQuantity = (int)(spawnVolume * density);
            float radiusDiff = outerRadius - innerRadius;


            Vector3 lastSegmentMidpoint = Vector3.zero;
            float lastSegmentMaxMidpointDist = 0f;
            Vector3? lastSegment = firstPoint.HasValue ? centerPoint - firstPoint : null;
            bool useWedgeAngler = firstPoint.HasValue;
            WedgeAnglerStuffs wedgeAngler = null;
            if (useWedgeAngler)
            {
                useWedgeAngler = WedgeAnglerStuffs.TryMake(lastSegment.Value, segmentStuffs.segment, out wedgeAngler);
            }
            if (useWedgeAngler)
            {
                lastSegmentMidpoint = (firstPoint.Value + centerPoint) / 2;
                lastSegmentMaxMidpointDist = Vector3.Distance(firstPoint.Value, centerPoint) / 2;
            }

            var newTube = Instantiate(TubePrefab, Vector3.zero, Quaternion.LookRotation(Vector3.forward));
            //newTube.transform.localScale = new Vector3(outerRadius, outerRadius, segmentStuffs.segment.magnitude);

            #region GenerateTubeMesh

            var mesh = new Mesh();
            var verticies = new Vector3[numTubeVertices * 2]; //Two sets of verticies, one for each side of the tube.

            Vector3 ihat;
            Vector3 jhat;
            //First circle
            if (useWedgeAngler)
            {
                //Warp the circle into a nice elipse matching (quite nicely) with the last circle in the tube.
                ihat = (1 / Mathf.Cos(wedgeAngler.wedgeAngle)) * -wedgeAngler.wedgePerpFromLast; //You can't, I guess, directly take a vector / a float.  I could do this per-component to save computing time, but where's the fun in that.
                jhat = wedgeAngler.wedgePlaneNormal;
            }
            else
            {
                ihat = segmentStuffs.rightVector;
                jhat = segmentStuffs.upVector;
            }

            var step = Mathf.PI * 2 / numTubeVertices;
            firstCircle = GizmosUtil.PointsOn3DArc(centerPoint, ihat, jhat, outerRadius, 0, Mathf.PI * 2 - step, step).ToArray(); //GizmosUtil is a custom helper utility.  I'm using it here to get a full circle of points.  I can certify that this works as I use the same method to generate a Gizmos Circle.
            var secondCircle = GizmosUtil.PointsOn3DArc(newPoint, segmentStuffs.rightVector, segmentStuffs.upVector, outerRadius, 0, Mathf.PI * 2 - step, step).ToArray(); //upVector and rightVector are analogous to "ihat" and "jhat".  I hadn't learned about transformation matricies yet when I wrote this, but know that this hat business is basically doing the same thing as one of those.
            mesh.vertices = firstCircle.Concat(secondCircle).ToArray();
            var triangles = new int[numTubeVertices * 6]; //Connecting the edge verticies into a series of triangles takes twice the number of triangles as verticies (I drew this out), and this needs thrice that for the three points to every triangle. Hence, six.
            for (int i = 0; i < numTubeVertices; i++)
            {
                int startIndex = i * 6;
                triangles[startIndex] = i; //Start with the iterated point (this will fall on first circle)
                triangles[startIndex + 1] = i + numTubeVertices; //Connect to same point on second circle;
                triangles[startIndex + 2] = i + numTubeVertices + 1; //Connect to next point over on second circle;

                triangles[startIndex + 3] = i; //Start again with iterated point
                triangles[startIndex + 4] = i + 1; //Connect to next point over on first circle
                triangles[startIndex + 5] = i + numTubeVertices + 1;//Connect to point corresponding to this next point, but on second circle
                if (triangles[startIndex + 4] >= numTubeVertices) triangles[startIndex + 4] -= numTubeVertices;
                if (triangles[startIndex + 5] >= mesh.vertices.Length) triangles[startIndex + 5] -= numTubeVertices;
                if (triangles[startIndex + 2] >= mesh.vertices.Length) triangles[startIndex + 2] -= numTubeVertices;
            }
            mesh.triangles = triangles;

            mesh.RecalculateNormals();

            newTube.GetComponent<MeshFilter>().mesh = mesh;
            TubeObjects.Add(newTube);
            #endregion

            Vector2 point;
            float dist;
            Vector3 asteroidPoint;
            Vector3 localLinePoint = Vector3.zero;
            float localLineDist;
            for (int asteroid = 0; asteroid < spawnQuantity; asteroid++)
            {
                do
                {
                    point = Random.insideUnitCircle * radiusDiff;
                    point += point.normalized * innerRadius;
                    dist = Random.Range(0, segmentLength);
                    asteroidPoint = centerPoint + point.x * segmentStuffs.rightVector + point.y * segmentStuffs.upVector + dist * segmentStuffs.segmentDir;
                    if (useWedgeAngler)
                    {
                        localLinePoint = asteroidPoint - lastSegmentMidpoint;
                    }
                } while (useWedgeAngler &&  //This while will keep re-guessing points (if wedge angler is being used) until one lands outside of the cutout zone.
                        (localLineDist = Vector3.Dot(localLinePoint, wedgeAngler.lastSegmentDir)) < lastSegmentMaxMidpointDist &&
                        (Vector3.Distance(asteroidPoint, localLinePoint + localLineDist * wedgeAngler.lastSegmentDir) < outerRadius)
                //find point nearest asteroidPoint on lastSegment line.  If A) within the bounds of that segmentStuffs.segment and B) within outerRadius of that segmentStuffs.segment, try again.
                );
                AsteroidAt(asteroidPoint);
            }

            if (useWedgeAngler) //Spawn asteroids in spherical corner wedge (in a shape like a slice of an orange)
            {
                float volume = outerSphereVolume * wedgeAngler.wedgeAngle / Mathf.PI * 0.5f;
                int wedgeSpawnQuantity = (int)(volume * density);
                for (int asteroid = 0; asteroid < wedgeSpawnQuantity; asteroid++)
                {
                    float randihatAngle = Random.Range(0f, wedgeAngler.wedgeAngle);
                    Vector3 randihat = wedgeAngler.wedgePerpFromLast * Mathf.Cos(randihatAngle) + wedgeAngler.lastSegmentDir * Mathf.Sin(randihatAngle);
                    asteroidPoint = GizmosUtil.PointOn3DCircle(Vector3.zero, randihat, wedgeAngler.wedgePlaneNormal, Random.Range(0, radiusDiff), Random.Range(-Mathf.PI * 0.5f, Mathf.PI * 0.5f));
                    asteroidPoint = centerPoint + asteroidPoint + asteroidPoint.normalized * innerRadius;
                    AsteroidAt(asteroidPoint);
                }
            }
        }
    }

    private void AsteroidAt(Vector3 asteroidPoint)
    {

        if (asteroidModels == null || asteroidModels.Length == 0)
        {
            Debug.LogError("No Asteroid Models");
            return;
        }
        Instantiate(asteroidModels[Random.Range(0, asteroidModels.Length - 1)], asteroidPoint, Quaternion.Euler(Vector3.forward));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3[] waypoints = pathManager.Waypoints?.ToArray() ?? TestWaypoints;
        if (waypoints.Length > 1)
        {
            for (int i = 1; i < waypoints.Length; i++)
            {
                Vector3 centerPoint = waypoints[i - 1];
                Vector3 newPoint = waypoints[i];
                using (new GizmosColor(Color.green))
                {
                    Gizmos.DrawLine(centerPoint, newPoint);
                }

                SegmentStuffs segmentStuffs = new SegmentStuffs(centerPoint, newPoint);

                bool useWedgeAngler = i >= 2;
                Vector3? firstPoint;
                if (useWedgeAngler)
                {
                    firstPoint = waypoints[i - 2];
                }
                else
                {
                    firstPoint = null;
                }

                Vector3 segment = newPoint - centerPoint;
                WedgeAnglerStuffs wedgeAngler = null;
                if (useWedgeAngler)
                {
                    var lastSegment = centerPoint - firstPoint.Value;
                    useWedgeAngler = WedgeAnglerStuffs.TryMake(lastSegment, segment, out wedgeAngler);
                }

                if (enableTubeGizmo)
                {
                    using (new GizmosColor(new Color(1, 1f, 0)))
                    {
                        Gizmos.DrawLine(newPoint, newPoint + segmentStuffs.upVector);
                        Gizmos.DrawLine(newPoint, newPoint + segmentStuffs.rightVector);

                        foreach (var radius in new[] { innerRadius, outerRadius })
                        {
                            foreach (var center in new[] { newPoint, centerPoint })
                                GizmosUtil.DrawCircle(center, segmentStuffs.rightVector, segmentStuffs.upVector, radius);
                        }

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += Mathf.PI * 2 / numTubeLines)
                        {
                            Vector3 relativeVector = GizmosUtil.PointOn3DCircle(Vector3.zero, segmentStuffs.rightVector, segmentStuffs.upVector, outerRadius, theta);
                            Gizmos.DrawLine(centerPoint + relativeVector, newPoint + relativeVector);
                        }
                        if (useWedgeAngler)
                        {
                            using (new GizmosColor(new Color(1, 0.5f, 0)))
                            {
                                for (float bandPos = -1; bandPos <= 1; bandPos += 2f / numCornerBands)
                                {
                                    Vector3 bandCenter = centerPoint + wedgeAngler.wedgePlaneNormal * bandPos * outerRadius;
                                    Gizmos.DrawSphere(bandCenter, 0.2f);
                                    float bandRadius = Mathf.Sqrt(Mathf.Pow(outerRadius, 2) - Mathf.Pow(bandPos * outerRadius, 2));
                                    GizmosUtil.DrawArc(
                                        centerPoint: bandCenter,
                                        ihat: wedgeAngler.wedgePerpFromLast,
                                        jhat: wedgeAngler.lastSegmentDir,
                                        radius: bandRadius,
                                        startAngle: 0,
                                        stopAngle: wedgeAngler.wedgeAngle);
                                }
                                GizmosUtil.DrawArcDegrees(centerPoint, wedgeAngler.wedgePerpFromLast, wedgeAngler.wedgePlaneNormal, outerRadius, 90, 270);
                                var cutback = outerRadius * Mathf.Tan(wedgeAngler.wedgeAngle / 2) * wedgeAngler.lastSegmentDir;
                                for (float theta = Mathf.PI * 0.5f; theta <= 1.5f * Mathf.PI; theta += 2 * Mathf.PI / numTubeLines)
                                {
                                    Vector3 point = centerPoint + GizmosUtil.PointOn3DCircle(Vector3.zero, wedgeAngler.wedgePerpFromLast, wedgeAngler.wedgePlaneNormal, outerRadius, theta);
                                    Gizmos.DrawLine(point, point - cutback);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

class SegmentStuffs
{
    public SegmentStuffs(Vector3 start, Vector3 end)
    {
        segment = end - start;
        segmentDir = segment.normalized;
        float perpZ = (-segmentDir.x * 0 - segmentDir.y * 1) / segmentDir.z;
        upVector = new Vector3(0, 1, perpZ);
        upVector /= upVector.magnitude;
        rightVector = Vector3.Cross(segmentDir, upVector);
        rightVector /= rightVector.magnitude;
    }
    public Vector3 segment { get; set; }
    public Vector3 segmentDir { get; set; }
    public Vector3 upVector { get; set; }
    public Vector3 rightVector { get; set; }
}

class WedgeAnglerStuffs
{
    public static bool TryMake(Vector3 lastSegment, Vector3 segment, out WedgeAnglerStuffs wedgeAnglerStuffs)
    {
        float wedgeAngle = Vector3.Angle(segment, lastSegment) * Mathf.Deg2Rad;
        if (wedgeAngle != 0)
        {

            Vector3 wedgePlaneNormal = Vector3.Cross(segment, lastSegment);
            wedgePlaneNormal /= wedgePlaneNormal.magnitude;
            wedgeAnglerStuffs = new WedgeAnglerStuffs()
            {
                lastSegment = lastSegment,
                lastSegmentDir = lastSegment.normalized,
                wedgePlaneNormal = wedgePlaneNormal,
                wedgePerpFromLast = Vector3.Cross(wedgePlaneNormal, lastSegment),
                wedgeAngle = wedgeAngle
            };
            wedgeAnglerStuffs.wedgePerpFromLast /= wedgeAnglerStuffs.wedgePerpFromLast.magnitude;
            return true;
        }
        else
        {
            wedgeAnglerStuffs = null;
            return false;
        }

    }
    public Vector3 lastSegment { get; set; }
    public Vector3 lastSegmentDir { get; set; }
    public Vector3 wedgePlaneNormal { get; set; }
    public Vector3 wedgePerpFromLast { get; set; }
    public float wedgeAngle { get; set; }
}