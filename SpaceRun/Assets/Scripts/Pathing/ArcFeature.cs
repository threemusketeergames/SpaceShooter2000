using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcFeature : PathFeature
{
    [Range(0, 5)]
    public float windiness = 1.5f;
    [Range(0,10000)]
    public float gradualness = 1000f;
    public override IEnumerator<Vector3> GetGenerator(Vector3 startPoint, Vector3 startDirection, float stepDist, float difficulty)
    {
        float arcDegrees = windiness*difficulty;
        float arcRadians = arcDegrees * (Mathf.PI / 180);
        float radius = gradualness/difficulty;

        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);
        float perpZ = (-startDirection.x * randX - startDirection.y * randY) / startDirection.z; //Perpendicular to startDirection, ensures vector's random direction points "orthogonally" away from start direction, which is good if you couldn't tell.
        Vector3 turnVector = new Vector3(randX, randY, perpZ).normalized;
        Vector3 arcCenter = startPoint + turnVector * radius;

        float radianStep = stepDist / radius;  //Radians
        float endRadians = 1.5f * Mathf.PI + arcRadians; //We start the circle at 270 degrees, or 1.5pi radians, aka the bottom of the circle.  This makes the arc start where it should.
        for(float a = 1.5f*Mathf.PI+radianStep; a <= endRadians; a+=radianStep)
        {
            yield return arcCenter + startDirection * Mathf.Cos(a) * radius + turnVector * Mathf.Sin(a) * radius;
        }
    }
}
