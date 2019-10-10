using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Pixelplacement/iTweenPath")]
public class ItweenPath : MonoBehaviour
{
    public string pathName = "";
    public Color pathColor = Color.cyan;
    public List<Vector3> nodes = new List<Vector3>() { Vector3.zero, Vector3.zero };
    public int nodeCount;
    public static Dictionary<string, ItweenPath> paths = new Dictionary<string, ItweenPath>();
    public bool initialized = false;
    public string initialName = "";
    public bool pathVisible = true;

    void OnEnable()
    {
        if (!paths.ContainsKey(pathName))
        {
            paths.Add(pathName.ToLower(), this);
        }
    }

    void OnDisable()
    {
        paths.Remove(pathName.ToLower());
    }

    void OnDrawGizmosSelected()
    {
        if (pathVisible)
        {
            if (nodes.Count > 0)
            {
                //iTween.DrawPath(nodes.ToArray(), pathColor);
                Vector3[] globalPoints = new Vector3[nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                {
                    globalPoints[i] = gameObject.transform.TransformPoint(nodes[i]);
                }

                iTween.DrawPath(globalPoints, pathColor);
            }
        }
    }

    /// <summary>
    /// Returns the visually edited path as a Vector3 array.
    /// </summary>
    /// <param name="requestedName">
    /// A <see cref="System.String"/> the requested name of a path.
    /// </param>
    /// <returns>
    /// A <see cref="Vector3[]"/>
    /// </returns>
    public static Vector3[] GetPath(string requestedName)
    {
        requestedName = requestedName.ToLower();
        if (paths.ContainsKey(requestedName))
        {
            //return paths[requestedName].nodes.ToArray();
            ItweenPath path = paths[requestedName];
            Vector3[] globalPoints = new Vector3[path.nodes.Count];
            for (int i = 0; i < path.nodes.Count; i++)
            {
                globalPoints[i] = path.gameObject.transform.TransformPoint(path.nodes[i]);
            }

            return globalPoints;
        }
        else
        {
            Debug.Log("No path with that name (" + requestedName + ") exists! Are you sure you wrote it correctly?");
            return null;
        }
    }

    /// <summary>
    /// Returns the reversed visually edited path as a Vector3 array.
    /// </summary>
    /// <param name="requestedName">
    /// A <see cref="System.String"/> the requested name of a path.
    /// </param>
    /// <returns>
    /// A <see cref="Vector3[]"/>
    /// </returns>
    public static Vector3[] GetPathReversed(string requestedName)
    {
        requestedName = requestedName.ToLower();
        if (paths.ContainsKey(requestedName))
        {
            List<Vector3> revNodes = paths[requestedName].nodes.GetRange(0, paths[requestedName].nodes.Count);
            revNodes.Reverse();
            return revNodes.ToArray();
        }
        else
        {
            Debug.Log("No path with that name (" + requestedName + ") exists! Are you sure you wrote it correctly?");
            return null;
        }
    }
}