using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// In slots/sec
    /// </summary>
	public float speed;
    public float acceleration;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	 
	private float nextFire;

    Rigidbody Rigidbody;
    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
   
    void Update ()
	{
		if (Input.GetButton("Fire1") && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().Play ();
		}
	}

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

        Rigidbody.velocity = new Vector3(moveHorizontal, 0, moveVertical);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, boundary.xMin, boundary.xMax), 0, Mathf.Clamp(transform.position.z, boundary.zMin, boundary.xMax));
        

        //Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
        //GetComponent<Rigidbody>().velocity = movement * speed;

        //GetComponent<Rigidbody>().position = new Vector3
        //(
        //	Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
        //	0.0f, 
        //	Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
        //);

        //GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
}
