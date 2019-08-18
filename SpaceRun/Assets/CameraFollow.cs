using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform player;
    Vector3 relativePosition;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        relativePosition = transform.position - player.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player==null) return;
        transform.position = player.position + relativePosition;
    }
}
