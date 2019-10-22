using System.Linq;
using UnityEngine;

public class PathSpawnManager : MonoBehaviour
{
    private PathManager pathManager;
    public float innerRadius = 6f;
    public float outerRadius = 10f;

    public bool enableTubeGizmo = true;

    public int numCornerBands = 10;
    public int numTubeLines = 8;

    public PathManager PathManager { get => pathManager ?? (pathManager = GetComponent<PathManager>()); set => pathManager = value; }

    private void Start()
    {
        // Use this for initialization
        for (int i = 1; i < PathManager.Waypoints.Count; i++)
        {
            SpawnForWaypoint(i);
        }
    }


    void SpawnForWaypoint(int index)
    {
        if (index > 0)
        {
            SpawnSegmentInfo ssi = new SpawnSegmentInfo();
            if (index > 1)
            {
                ssi.firstPoint = PathManager.Waypoints.ElementAt(index - 2);
            }
            else
            {
                ssi.firstPoint = null;
            }

            ssi.centerPoint = PathManager.Waypoints.ElementAt(index - 1);
            ssi.newPoint = PathManager.Waypoints.ElementAt(index);

            ssi.mainSegment = new LineSegmentInfo(ssi.centerPoint, ssi.newPoint);
            ssi.lastSegment = ssi.firstPoint.HasValue ? new LineSegmentInfo(ssi.centerPoint, ssi.firstPoint.Value) : null;
            ssi.useWedgeAngler = ssi.firstPoint.HasValue; //First check to use wedgeAngler:  Does first point exist
            if (ssi.useWedgeAngler)
            {
                ssi.useWedgeAngler = WedgeAngler.TryMake(ssi.lastSegment.dir, ssi.mainSegment.seg, out WedgeAngler wedgeAngler); //Second check to use wedgeAngler (done inside the function):  do the three points actually form an angle?
                ssi.wedgeAngler = wedgeAngler;
            }
            SendMessage("SpawnForSegment", ssi);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3[] waypoints = PathManager.Waypoints.ToArray();
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

                LineSegmentInfo mainSegment = new LineSegmentInfo(centerPoint, newPoint);

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

                LineSegmentInfo lastSegment = useWedgeAngler ? new LineSegmentInfo(firstPoint.Value,centerPoint) : (LineSegmentInfo) null;
                WedgeAngler wedgeAngler = null;
                if (useWedgeAngler)
                {
                    useWedgeAngler = WedgeAngler.TryMake(lastSegment.seg, mainSegment.seg, out wedgeAngler);
                }

                if (enableTubeGizmo)
                {
                    using (new GizmosColor(new Color(1, 1f, 0)))
                    {
                        Gizmos.DrawLine(newPoint, newPoint + mainSegment.upVector);
                        Gizmos.DrawLine(newPoint, newPoint + mainSegment.rightVector);

                        foreach (var radius in new[] { innerRadius, outerRadius })
                        {
                            foreach (var center in new[] { newPoint, centerPoint })
                                GizmosUtil.DrawCircle(center, mainSegment.rightVector, mainSegment.upVector, radius);
                        }

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += Mathf.PI * 2 / numTubeLines)
                        {
                            Vector3 relativeVector = GizmosUtil.PointOn3DCircle(Vector3.zero, mainSegment.rightVector, mainSegment.upVector, outerRadius, theta);
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
                                        jhat: lastSegment.dir,
                                        radius: bandRadius,
                                        startAngle: 0,
                                        stopAngle: wedgeAngler.wedgeAngle);
                                }
                                //GizmosUtil.DrawArcDegrees(centerPoint, wedgeAngler.wedgePerpFromLast, wedgeAngler.wedgePlaneNormal, outerRadius, 90, 270);
                                //var cutback = outerRadius * Mathf.Tan(wedgeAngler.wedgeAngle / 2) * lastSegment.dir;
                                //for (float theta = Mathf.PI * 0.5f; theta <= 1.5f * Mathf.PI; theta += 2 * Mathf.PI / numTubeLines)
                                //{
                                //    Vector3 point = centerPoint + GizmosUtil.PointOn3DCircle(Vector3.zero, wedgeAngler.wedgePerpFromLast, wedgeAngler.wedgePlaneNormal, outerRadius, theta);
                                //    Gizmos.DrawLine(point, point - cutback);
                                //}
                            }
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class LineSegmentInfo
{
    public LineSegmentInfo(Vector3 start, Vector3 end)
    {
        seg = end - start;
        dir = seg.normalized;
        float perpZ = (-dir.x * 0 - dir.y * 1) / dir.z;
        upVector = new Vector3(0, 1, perpZ);
        upVector /= upVector.magnitude;
        rightVector = Vector3.Cross(dir, upVector);
        rightVector /= rightVector.magnitude;
    }
    public Vector3 seg { get; set; }
    public Vector3 dir { get; set; }
    public Vector3 upVector { get; set; }
    public Vector3 rightVector { get; set; }
}

public class WedgeAngler
{
    public static bool TryMake(Vector3 lastSegment, Vector3 segment, out WedgeAngler wedgeAngler)
    {
        float wedgeAngle = Vector3.Angle(segment, lastSegment) * Mathf.Deg2Rad;
        if (wedgeAngle != 0)
        {

            Vector3 wedgePlaneNormal = Vector3.Cross(segment, lastSegment);
            wedgePlaneNormal /= wedgePlaneNormal.magnitude; //Normalize manually (for some reason the built-in function wouldn't work)
            wedgeAngler = new WedgeAngler()
            {
                wedgePlaneNormal = wedgePlaneNormal,
                wedgePerpFromLast = Vector3.Cross(wedgePlaneNormal, lastSegment),
                wedgeAngle = wedgeAngle
            };
            wedgeAngler.wedgePerpFromLast /= wedgeAngler.wedgePerpFromLast.magnitude;
            return true;
        }
        else
        {
            wedgeAngler = null;
            return false;
        }

    }
    public Vector3 wedgePlaneNormal { get; set; }
    public Vector3 wedgePerpFromLast { get; set; }
    public float wedgeAngle { get; set; }
}
public class SpawnSegmentInfo
{
    public Vector3? firstPoint { get; set; }
    public Vector3 centerPoint { get; set; }
    public Vector3 newPoint { get; set; }
    public LineSegmentInfo mainSegment { get; set; }
    public LineSegmentInfo lastSegment { get; set; }
    public bool useWedgeAngler { get; set; }
    public WedgeAngler wedgeAngler { get; set; }
}