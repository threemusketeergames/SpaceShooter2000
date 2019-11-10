using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawning : MonoBehaviour
{
    public Vector3 CurrentPoint; //The current point of asteroid spawining
    public GameObject Asteroid;
    public GameObject EnemyShip;
    public Quaternion Rotation; //Roation at spawn
    public GameObject[] hazards; //Options for hazards
    public int hazardCount; //How many per segment

    public void SpawnForSegment(SpawnSegementInfo ssi)
    {
        for (int i = 0; i < hazardCount; i++)
        {
            GameObject hazard = hazards[Random.Range(0, hazards.Length)];
            CurrentPoint = GizmosUtil.PointOn3DCircle(ssi.mainSegment.dir *  Random.Range(0,ssi.mainSegment.length), ihat, jhat, this.GetComponent<GeometryGizmo>().innerRadius, angle);
            Instantiate(Asteroid, CurrentPoint, Rotation);
        }

    }
}
