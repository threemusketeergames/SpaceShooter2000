using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class PathFeature: MonoBehaviour
{
    /// <summary>
    /// Generate a series of Vector3 points of the implementor's Feature type. It can be of any length or number of points, although a good standard is 1 < number < 100 or so.
    /// </summary>
    /// <param name="startPoint">Where to generate points relative to - this is the stopping point of the last feature, or the origin.</param>
    /// <param name="startDirection">The direction the path left off at - use this to base every action off of.</param>
    /// <param name="stepDist">How far apart to make the vectors along the feature, in units.</param>
    /// <param name="difficulty">A number (with scale 1-100) for implementors to base relative difficult of the generate paths off of. 
    /// The idea is, if for example the implementor is a turn feature, a greater difficulty shuold signal a sharper turn.</param>
    /// <returns>An IEnumerable which can be read from point-by-point until the feature is complete.</returns>
    public abstract IEnumerator<Vector3> GetGenerator(Vector3 startPoint, Vector3 startDirection, float stepDist, float difficulty);
}
