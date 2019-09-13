using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeometryGizmo : MonoBehaviour
{
    public Vector3[] TestWaypoints = new Vector3[]
    {
        Vector3.zero,
        5 * Vector3.forward
    };
    public float innerRadius;
    public float outerRadius;
    public float density;

    public bool enableTubeGizmo = true;
    public bool enableAsteroidSpawns = true;

    public int numCornerBands = 10;
    public int numTubeLines = 8;
    private void Start()
    {
        TestWaypoints = GameController.Instance.pathManager.Waypoints.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if (TestWaypoints.Length > 1)
        {
            float outerSphereVolume = 4 / 3 * Mathf.PI * Mathf.Pow(outerRadius, 3);
            for (int i = 1; i < TestWaypoints.Length; i++)
            {
                using (new GizmosColor(Color.green))
                {
                    Gizmos.DrawLine(TestWaypoints[i - 1], TestWaypoints[i]);
                }

                Vector3 segment = TestWaypoints[i] - TestWaypoints[i - 1];
                Vector3 segmentDir = segment.normalized;
                float perpZ = (-segmentDir.x * 0 - segmentDir.y * 1) / segmentDir.z;
                Vector3 upVector = new Vector3(0, 1, perpZ);
                upVector.Normalize();
                Vector3 rightVector = Vector3.Cross(segmentDir, upVector);
                rightVector.Normalize();


                float wedgeAngle = 0;
                bool useWedgeAngler = i >= 2;
                Vector3 lastSegment;
                Vector3 lastSegmentDir = Vector3.zero;
                Vector3 wedgePlaneNormal = Vector3.zero;
                Vector3 wedgePerpFromLast = Vector3.zero;
                Vector3 wedgeCenter = TestWaypoints[i - 1];
                if (useWedgeAngler)
                {
                    lastSegment = wedgeCenter - TestWaypoints[i - 2];
                    lastSegmentDir = lastSegment.normalized;
                    wedgeAngle = Vector3.Angle(segment, lastSegment) * Mathf.Deg2Rad;
                    if (wedgeAngle != 0)
                    {
                        wedgePlaneNormal = Vector3.Cross(segment, lastSegment);
                        wedgePlaneNormal.Normalize();
                        wedgePerpFromLast = Vector3.Cross(wedgePlaneNormal, lastSegment);
                        wedgePerpFromLast.Normalize();
                    }
                    else
                    {
                        useWedgeAngler = false;
                    }
                }



                if (enableTubeGizmo)
                {
                    using (new GizmosColor(new Color(1, 1f, 0)))
                    {
                        Gizmos.DrawLine(TestWaypoints[i], TestWaypoints[i] + upVector);
                        Gizmos.DrawLine(TestWaypoints[i], TestWaypoints[i] + rightVector);

                        foreach (var radius in new[] { innerRadius, outerRadius })
                        {
                            foreach (var center in new[] { TestWaypoints[i], TestWaypoints[i - 1] })
                                GizmosUtil.DrawCircle(center, rightVector, upVector, radius);
                        }

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += Mathf.PI * 2 / numTubeLines)
                        {
                            Vector3 relativeVector = GizmosUtil.PointOn3DCircle(Vector3.zero, rightVector, upVector, outerRadius, theta);
                            Gizmos.DrawLine(TestWaypoints[i - 1] + relativeVector, TestWaypoints[i] + relativeVector);
                        }
                        if (useWedgeAngler)
                        {
                            using (new GizmosColor(new Color(1, 0.5f, 0)))
                            {
                                for (float bandPos = -1; bandPos <= 1; bandPos += 2f / numCornerBands)
                                {
                                    Vector3 bandCenter = wedgeCenter + wedgePlaneNormal * bandPos * outerRadius;
                                    Gizmos.DrawSphere(bandCenter, 0.2f);
                                    float bandRadius = Mathf.Sqrt(Mathf.Pow(outerRadius, 2) - Mathf.Pow(bandPos * outerRadius, 2));
                                    GizmosUtil.DrawArc(
                                        centerPoint: bandCenter,
                                        ihat: wedgePerpFromLast,
                                        jhat: lastSegmentDir,
                                        radius: bandRadius,
                                        startAngle: 0,
                                        stopAngle: wedgeAngle);
                                }
                                GizmosUtil.DrawArcDegrees(wedgeCenter, wedgePerpFromLast, wedgePlaneNormal, outerRadius, 90, 270);
                                var cutback = outerRadius * Mathf.Tan(wedgeAngle / 2) * lastSegmentDir;
                                for (float theta = Mathf.PI*0.5f; theta <= 1.5f * Mathf.PI; theta += 2*Mathf.PI / numTubeLines)
                                {
                                    Vector3 point = TestWaypoints[i - 1] + GizmosUtil.PointOn3DCircle(Vector3.zero, wedgePerpFromLast, wedgePlaneNormal, outerRadius, theta);
                                    Gizmos.DrawLine(point,point - cutback);
                                }
                            }
                        }
                    }
                }

                if (enableAsteroidSpawns)
                {
                    float segmentLength = segment.magnitude;
                    float hollowedCircleArea = 2 * Mathf.PI * (Mathf.Pow(outerRadius, 2) - Mathf.Pow(innerRadius, 2)); // Subtract big circle by little circle.  I factored out the 2pi like a total math pro.
                    float spawnVolume = hollowedCircleArea * segmentLength;
                    int spawnQuantity = (int)(spawnVolume * density);
                    float radiusDiff = outerRadius - innerRadius;
                    Vector3 startPoint = TestWaypoints[i - 1];

                    Vector3 lastSegmentMidpoint = Vector3.zero;
                    float lastSegmentMaxMidpointDist = 0f;
                    if (useWedgeAngler)
                    {
                        lastSegmentMidpoint = (TestWaypoints[i - 2] + TestWaypoints[i - 1]) / 2;
                        lastSegmentMaxMidpointDist = Vector3.Distance(TestWaypoints[i - 2], TestWaypoints[i - 1]) / 2;
                    }

                    using (new GizmosColor(new Color(0, 0.75f, 1)))
                    {
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
                                asteroidPoint = startPoint + point.x * rightVector + point.y * upVector + dist * segmentDir;
                                if (useWedgeAngler)
                                {
                                    localLinePoint = asteroidPoint - lastSegmentMidpoint;
                                }
                            } while (useWedgeAngler &&  //This while will keep re-guessing points (if wedge angler is being used) until one lands outside of the cutout zone.
                                    (localLineDist = Vector3.Dot(localLinePoint,lastSegmentDir))<lastSegmentMaxMidpointDist &&
                                    (Vector3.Distance(asteroidPoint, localLinePoint + localLineDist*lastSegmentDir)<outerRadius)
                                //find point nearest asteroidPoint on lastSegment line.  If A) within the bounds of that segment and B) within outerRadius of that segment, try again.
                            ) ;
                            Gizmos.DrawSphere(asteroidPoint, 0.25f);
                        }
                    }
                    if (useWedgeAngler) //Spawn asteroids in spherical corner wedge (in a shape like a slice of an orange)
                    {
                        using (new GizmosColor(1, 0.5f, 0))
                        {
                            float volume = outerSphereVolume * wedgeAngle / Mathf.PI * 0.5f;
                            int wedgeSpawnQuantity = (int)(volume * density);
                            for(int asteroid = 0; asteroid<wedgeSpawnQuantity; asteroid++)
                            {
                                float randihatAngle = Random.Range(0f, wedgeAngle);
                                Vector3 randihat = wedgePerpFromLast * Mathf.Cos(randihatAngle) + lastSegmentDir * Mathf.Sin(randihatAngle);
                                Vector3 point = GizmosUtil.PointOn3DCircle(Vector3.zero, randihat, wedgePlaneNormal, Random.Range(0, radiusDiff), Random.Range(-Mathf.PI * 0.5f, Mathf.PI * 0.5f));
                                point = wedgeCenter + point + point.normalized * innerRadius;
                                Gizmos.DrawSphere(point, 0.25f);
                            }
                        }
                    }
                }
            }
        }
    }
}

public class GizmosColor : IDisposable
{
    Color oldColor;
    public GizmosColor(Color color)
    {
        oldColor = Gizmos.color;
        Gizmos.color = color;
    }
    public GizmosColor(float r, float g, float b): this(new Color(r,g, b))
    {

    }

    public void Dispose()
    {
        Gizmos.color = oldColor;
    }
}

public static class GizmosUtil
{
    /// <summary>
    /// Draw a circle, as described by the vectors ihat and jhat, which should be perpendicular and normalized
    /// </summary>
    /// <param name="ihat">The x-axis unit vector. In the standard xy plane, this would be (0,1)</param>
    /// <param name="jhat">The y-axis unit vector. In the standard xy plane, this would be (1,0)</param>
    /// <param name="radius">Radius of the circle to draw.</param>
    public static void DrawCircle(Vector3 centerPoint, Vector3 ihat, Vector3 jhat, float radius)
    {
        DrawArc(centerPoint, ihat, jhat, radius, 0, 2 * Mathf.PI);
    }

    public static void DrawArcDegrees(Vector3 centerPoint, Vector3 ihat, Vector3 jhat, float radius, float startAngle, float stopAngle, float? step = null)
    {
        if (step == null)
        {
            step = 0.1f;
        }
        else
        {
            step *= Mathf.Deg2Rad;
        }
        startAngle *= Mathf.Deg2Rad;
        stopAngle *= Mathf.Deg2Rad;
        DrawArc(centerPoint, ihat, jhat, radius, startAngle , stopAngle, step.Value);
    }

    public static void DrawArc(Vector3 centerPoint, Vector3 ihat, Vector3 jhat, float radius, float startAngle, float stopAngle, float step = 0.1f)
    {
        if (startAngle > stopAngle)
        {
            float swap = stopAngle;
            stopAngle = startAngle;
            startAngle = swap;
        }
        Vector3 point;
        Vector3 startPoint = PointOn3DCircle(centerPoint, ihat, jhat, radius, startAngle); //Start at jhat, which points toward where angles start in the unit circle.
        Vector3 lastPoint = startPoint;
        for (float theta = startAngle + step; theta < stopAngle; theta += step)
        {
            point = PointOn3DCircle(centerPoint, ihat, jhat, radius, theta);
            Gizmos.DrawLine(lastPoint, point);
            lastPoint = point;
        }
        Gizmos.DrawLine(lastPoint, PointOn3DCircle(centerPoint, ihat, jhat, radius, stopAngle)); //Finish the arc;
    }

    public static Vector3 PointOn3DCircle(Vector3 centerPoint, Vector3 ihat, Vector3 jhat, float radius, float theta)
    {
        return centerPoint + ihat * radius * Mathf.Cos(theta) + jhat * radius * Mathf.Sin(theta); //Upon later revisitation, this stuff would've less confusing if I'd used matrices...
    }

}

