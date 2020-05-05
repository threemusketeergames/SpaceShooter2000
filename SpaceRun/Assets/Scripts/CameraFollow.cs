using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform player;
    public float positionDamping = 1.5f;
    Vector3 relativePosition;
    
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        relativePosition = transform.position - player.position;
       
    }



    void LateUpdate()
    {
        if (!player)
            return;

        Vector3 wantedPosition = player.position + player.right * relativePosition.x + player.up * relativePosition.y + player.forward * relativePosition.z;
        
        Vector3 currentPosition = transform.position;

        currentPosition = Vector3.Lerp(currentPosition, wantedPosition, positionDamping* Time.deltaTime);
        
        Vector3 deltaVector = currentPosition - player.position;
        currentPosition = Vector3.Normalize(deltaVector) * relativePosition.magnitude + player.position; 

        transform.position = currentPosition;
        transform.LookAt(player.position, player.up);

    }
}
