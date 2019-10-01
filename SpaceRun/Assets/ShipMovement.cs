using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed;
    public float rotatespeed;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
       //rb.velocity = Vector3.forward * moveSpeed;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!this.GetComponent<PlayerController>().gamecontrollerscript.GetComponent<LightSpeed>().LighSpeedActive)
        {
            //Turning left and right
            rb.AddRelativeTorque(Vector3.up * Input.GetAxis("Horizontal") * rotatespeed);
            rb.AddRelativeTorque(Vector3.left * Input.GetAxis("Vertical") * rotatespeed);

            //Constant moving foward
            rb.velocity = transform.forward * moveSpeed;
        }
       
    }
}
