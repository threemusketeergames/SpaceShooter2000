using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform player;
    // The distance in the x-z plane to the player
    public float distance = 10.0f;
    // the height we want the camera to be above the player
    public float height = 5.0f;
    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }



    void LateUpdate()
    {
        // Early out if we don't have a player
        if (!player)
            return;


        // Calculate the current rotation angles
        float wantedRotationAngle = player.eulerAngles.y;
        float wantedHeight = player.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;
        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the player
        transform.position = player.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        // Always look at the player
        transform.LookAt(player);

        //float angle = 0f;
        //float radius = 0f;
        //// Update is called once per frame
        //void FixedUpdate()
        //{
        //    //if (player==null) return;
        //    //transform.position = player.position + relativePosition;
        //    //playerLocationHistory.Enqueue(player.position);
        //    Vector3 playerForward = player.forward;
        //    Vector3 oldCameraPointOnLine = Vector3.Project(transform.position, playerForward);
        //    Vector3 newCameraPointOnLine = player.position + playerForward * relativePosition.z;
        //    float angle = Vector3.SignedAngle(
        //        Vector3.Normalize(transform.position - oldCameraPointOnLine),
        //        player.up,
        //        playerForward);
        //    float radius = Vector3.Distance(
        //        oldCameraPointOnLine,
        //        transform.position);
        //    angle = Mathf.Lerp(angle, 0, 0.1f);
        //    radius = Mathf.Lerp(radius, relativePosition.y, 0.9f);
        //    Vector3 newCameraLocation = GizmosUtil.PointOn3DCircle(
        //        newCameraPointOnLine,
        //        player.right,
        //        player.up,
        //        radius,
        //        (angle + 90) * Mathf.PI / 180);
        //    transform.position = newCameraLocation;
        //    transform.LookAt(player, player.up);
        //}
    }
}
