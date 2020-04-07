using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform player;
    Vector3 relativePosition;
    //Queue<Vector3> playerLocationHistory;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        relativePosition = transform.position - player.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (player==null) return;
        //transform.position = player.position + relativePosition;
        //playerLocationHistory.Enqueue(player.position);
        Vector3 playerForward = player.forward;
        Vector3 cameraPointOnLine = player.position - playerForward * relativePosition.z;
        Vector3 cameraDirection = Vector3.Normalize(transform.position - cameraPointOnLine);
        float cameraAngle = Vector3.SignedAngle(player.up, cameraDirection, playerForward);
        float newAngle = Mathf.Lerp(cameraAngle, 0, 0.5f);
        Vector3 newCameraLocation = GizmosUtil.PointOn3DCircle(
            cameraPointOnLine,
            Vector3.Cross(playerForward, cameraDirection),
            cameraDirection,
            relativePosition.y,
            newAngle + 90);
        transform.position = newCameraLocation;
    }
}
