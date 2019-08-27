using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class StraightFeature : PathFeature
{

    public override IEnumerator<Vector3> GetGenerator(Vector3 startPoint, Vector3 startDirection, float stepDist, float difficulty)
    {
        float dist = 100 / difficulty;
        for (float i = stepDist; i <= dist; i += stepDist)
        {
            yield return startPoint + (startDirection * i);
        }
    }
}
