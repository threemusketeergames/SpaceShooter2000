using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawning : MonoBehaviour
{ 
    public GameObject[] hazards; //Options for hazards
    public int hazardCount; //How many per segment
    public PathSpawnManager psm;

    void Start()
    {
        psm = GetComponent<PathSpawnManager>();
    }

    public void SpawnForSegment(SpawnSegmentInfo ssi)
    {
        for (int i = 0; i < hazardCount; i++)
        {
            GameObject hazard = hazards[Random.Range(0, hazards.Length-1)];
            Vector3 CurrentPoint = ssi.centerPoint + GizmosUtil.PointOn3DCircle(ssi.mainSegment.dir *  Random.Range(0,ssi.mainSegment.length), ssi.mainSegment.rightVector, ssi.mainSegment.upVector, psm.innerRadius/2f, Random.Range(0,Mathf.PI*2f));
            Vector3 up = ssi.useWedgeAngler ? ssi.wedgeAngler.wedgePerpFromMain : ssi.mainSegment.upVector;
            Instantiate(hazard, CurrentPoint, Quaternion.LookRotation(-ssi.mainSegment.dir));
           // Vector3.RotateTowards()
        }

    }
}
