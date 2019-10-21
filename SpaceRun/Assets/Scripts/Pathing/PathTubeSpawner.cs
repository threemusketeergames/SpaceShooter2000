using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PathTubeSpawner : MonoBehaviour
{

    public GameObject TubePrefab;
    public List<GameObject> TubeObjects;
    public int numTubeVertices = 40;

    /// <summary>
    /// Path Spawn Manager
    /// </summary>
    PathSpawnManager psm;

    void Start()
    {
        psm = GetComponent<PathSpawnManager>();
    }
    void SpawnForSegment(SpawnSegmentInfo ssi)
    {
        var newTube = Instantiate(TubePrefab, Vector3.zero, Quaternion.LookRotation(Vector3.forward));
        //newTube.transform.localScale = new Vector3(outerRadius, outerRadius, segmentStuffs.segment.magnitude);

        #region GenerateTubeMesh

        var mesh = new Mesh();
        var verticies = new Vector3[numTubeVertices * 2]; //Two sets of verticies, one for each side of the tube.

        Vector3 ihat;
        Vector3 jhat;
        //First circle
        if (ssi.useWedgeAngler)
        {
            //Warp the circle into a nice elipse matching (quite nicely) with the last circle in the tube. The cheaty way to do this is to scale the basis vector to a length which isn't one.
            ihat = (1 / Mathf.Cos(ssi.wedgeAngler.wedgeAngle)) * -ssi.wedgeAngler.wedgePerpFromLast; //You can't, I guess, directly take a vector / a float.  I could do this per-component to save computing time, but where's the fun in that.
            jhat = ssi.wedgeAngler.wedgePlaneNormal;
        }
        else
        {
            ihat = ssi.mainSegment.rightVector;
            jhat = ssi.mainSegment.upVector;
        }

        var step = Mathf.PI * 2 / numTubeVertices;
        var firstCircle = GizmosUtil.PointsOn3DArc(ssi.centerPoint, ihat, jhat, psm.outerRadius, 0, Mathf.PI * 2 - step, step).ToArray(); //GizmosUtil is a custom helper utility.  I'm using it here to get a full circle of points.  I can certify that this works as I use the same method to generate a Gizmos Circle.
        var secondCircle = GizmosUtil.PointsOn3DArc(ssi.newPoint, ssi.mainSegment.rightVector, ssi.mainSegment.upVector, psm.outerRadius, 0, Mathf.PI * 2 - step, step).ToArray(); //upVector and rightVector are analogous to "ihat" and "jhat".  I hadn't learned about transformation matricies yet when I wrote this, but know that this hat business is basically doing the same thing as one of those.
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

    }

    //private void AsteroidAt(Vector3 asteroidPoint)
    //{

    //if (asteroidModels == null || asteroidModels.Length == 0)
    //    {
    //        Debug.LogError("No Asteroid Models");
    //        return;
    //    }
    //    Instantiate(asteroidModels[Random.Range(0, asteroidModels.Length - 1)], asteroidPoint, Quaternion.Euler(Vector3.forward));
    //}
}

