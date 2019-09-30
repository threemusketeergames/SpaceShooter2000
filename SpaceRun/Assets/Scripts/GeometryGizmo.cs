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

