using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed;
    public float rotatespeed;


    // Update is called once per frame
    void Update()
    {
        //Constant moving foward
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        //Turning left and right
        transform.Rotate(Vector3.up * Time.deltaTime * Input.GetAxis("Horizontal") * rotatespeed);
        transform.Rotate(Vector3.left * Time.deltaTime * Input.GetAxis("Vertical") * rotatespeed);
    }
}
