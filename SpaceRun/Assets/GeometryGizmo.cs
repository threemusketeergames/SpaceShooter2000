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
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if (TestWaypoints.Length > 1)
        {
            for (int i = 1; i < TestWaypoints.Length; i++)
            {
                using (new GizmosColor(Color.green))
                {
                    Gizmos.DrawLine(TestWaypoints[i - 1], TestWaypoints[i]);
                }

                Vector3 segment = TestWaypoints[i] - TestWaypoints[i - 1];
                Vector3 segmentDir = segment.normalized;
                float x = 0f;
                float y = 1f;
                float perpZ = (-segmentDir.x * x - segmentDir.y * y) / segmentDir.z;
                Vector3 upVector = new Vector3(x, y, perpZ);
                upVector.Normalize();
                Vector3 rightVector = Vector3.Cross(segmentDir, upVector);
                rightVector.Normalize();
                using (new GizmosColor(new Color(200, 100, 0)))
                {
                    Gizmos.DrawLine(TestWaypoints[i], TestWaypoints[i] + upVector);
                    Gizmos.DrawLine(TestWaypoints[i], TestWaypoints[i] + rightVector);
                    

                    float segmentLength = segment.magnitude;
                }
                //float hollowedCircleArea = 2 * Mathf.PI * outerRadius - 2 * Mathf.PI * innerRadius;
                //float spawnVolume = hollowedCircleArea * segmentLength;
                //int spawnQuantity = (int)(spawnVolume * density);
                //for (int asteroid = 0; asteroid < spawnQuantity; i++)
                //{

                //}
            }
        }
    }
}

public class GizmosColor : IDisposable{
    Color oldColor;
    public GizmosColor(Color color)
    {
        oldColor = Gizmos.color;
        Gizmos.color = color;
    }

    public void Dispose()
    {
        Gizmos.color = oldColor;
    }
}