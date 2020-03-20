using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed;
    public float rotatespeed;
    public Rigidbody rb;
    LightSpeed lightSpeed;

    private void Start()
    {
        lightSpeed = GameObject.FindWithTag("GameController").GetComponent<LightSpeed>();
        //rb.velocity = Vector3.forward * moveSpeed;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //!lightSpeed.LighSpeedActive
        if (true)
        {
            //Turning left and right
            rb.AddRelativeTorque(Vector3.up * Input.GetAxis("Horizontal") * rotatespeed);
            rb.AddRelativeTorque(Vector3.left * Input.GetAxis("Vertical") * rotatespeed);
            //Rotation
            float x;
            float z;
            x = Input.GetAxis("Verticle") * rotatespeed;
            z = Input.GetAxis("Horizontal") * rotatespeed;

       //    rb.gameObject.transform.rotation = Quaternion.Euler(x,0,z);
            //Constant moving foward
            rb.velocity = transform.forward * moveSpeed;



        }
        else
        {
            rb.velocity = Vector3.zero;
        }
       
    }
}
