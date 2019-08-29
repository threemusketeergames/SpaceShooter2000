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

    bool switchingSlots;
    Vector2 currentSlot;
    RocketSlotGrid RocketSlotGrid;
    Vector2 startPos;
    Vector2 targetPos;
    Vector2 targetDirection;
    float startTime;
    float distanceToTravel = 0f;
    public float moveSpeed = 3f;
    public GameObject Ship;



    Rigidbody Rigidbody;
    private void Start()
    {
        RocketSlotGrid = GameController.Instance.RocketSlotGrid;
        currentSlot = RocketSlotGrid.middle;
        currentSlot.x = Mathf.Ceil(currentSlot.x);
        currentSlot.y = Mathf.Ceil(currentSlot.y);
        Rigidbody = GetComponent<Rigidbody>();
    }
   
    void Update ()
	{



        //Roatation
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Rotate(0,0,10);
        
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            transform.Rotate(0, 0,-10);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Rotate(0, 0, -10);

        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            transform.Rotate(0, 0, +10);
        }

        if (Input.GetButton("Fire1") && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().Play ();
		}
        if (switchingSlots)
        {
            //We reference the current speed in this "distanceForStop" method, in case the ship doesn't make full speed before decelerating
            float distanceForStop = Mathf.Pow(Rigidbody.velocity.magnitude, 2) / (2 * acceleration); //This is what they call "Markerboard Math"
            float distanceFromStart = Vector2.Distance(startPos, transform.position);
            float distanceToTarget = distanceToTravel - distanceFromStart; //I do this instead of a Vector2.Distance here so that, should the ship overshoot, these values will go negative and bring it back. Or at least, that's the theory.
            //According to some really fancy-pants math, if we decellerate at a constant rate of 1 slot/sec, the distance we'd travel until being stopped is equal to our speed times our speed. 
            if (distanceForStop>=distanceToTarget)
            {
                Debug.Log("Decelerating");
                if (Mathf.Abs(distanceToTarget) <= 0.02)
                {
                    Rigidbody.velocity = Vector3.zero;
                    transform.position = targetPos;
                    switchingSlots = false;
                    Debug.Log("Arrived!");
                }
                else
                {
                    var decelSpeed = Mathf.Sqrt(2 * Mathf.Abs(distanceToTarget) * acceleration);//More Markerboard Math, Man. (this is actually the distanceForStop equation, solved for speed.
                    if (distanceToTarget < 0) decelSpeed *= -1;
                    SetRigidBodySpeed(decelSpeed);
                }
            }
            else
            {
                var accelSpeed = Mathf.Clamp((Time.time - startTime) * acceleration,0,speed);
                SetRigidBodySpeed(accelSpeed);
            }
        }
	}

    private void SetRigidBodySpeed(float setSpeed)
    {
        Rigidbody.velocity = targetDirection * setSpeed;
    }

    private void SwitchToSlot(Vector2 nextSlot)
    {
        if (currentSlot == nextSlot) return;
        startPos = transform.position;
        var relativePos = new Vector2(nextSlot.x-0.5f, nextSlot.y-0.5f) - RocketSlotGrid.middle; //Relative to middle of grid.  The -0.5's point the ship to the center of the slots
        targetPos = relativePos * RocketSlotGrid.slotSize;
        var positionDelta = targetPos - new Vector2(transform.position.x, transform.position.y);
        targetDirection = positionDelta / positionDelta.magnitude;
        distanceToTravel = positionDelta.magnitude;
        startTime = Time.time;
        currentSlot = nextSlot;
        Debug.Log($"Target Pos:  {targetPos} Direction:  {targetDirection}");
    }

 
	//void FixedUpdate ()
	//{
	//	float moveHorizontal = Input.GetAxis ("Horizontal");
	//	float moveVertical = Input.GetAxis ("Vertical");

 //       if (!switchingSlots)
 //       {
 //           Vector2 nextSlot = currentSlot;
 //           if (moveHorizontal >= 0.5)
 //           {
 //               nextSlot += Vector2.right;
 //           }else if (moveHorizontal <= -0.5)
 //           {
 //               nextSlot += Vector2.left;
 //           }
 //           if (moveVertical >= 0.5)
 //           {
 //               nextSlot += Vector2.up;
 //           }else if(moveVertical <= -0.5)
 //           {
 //               nextSlot += Vector2.down;
 //           }
 //           nextSlot.x = Mathf.Clamp(nextSlot.x, 0, RocketSlotGrid.dimensions.x);
 //           nextSlot.y = Mathf.Clamp(nextSlot.y, 0, RocketSlotGrid.dimensions.y);
 //           if (nextSlot != currentSlot)
 //           {
 //               Debug.Log("Switching Slots");
 //               switchingSlots = true;
 //               SwitchToSlot(nextSlot);
 //           }
 //       }
        
        //old script 
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
