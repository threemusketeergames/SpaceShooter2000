using UnityEngine;
using System.Collections;

public class EvasiveManeuver : MonoBehaviour
{
	//public Boundary boundary;
	public float tilt;
	public float dodge;
	public float smoothing;
	public Vector2 startWait;
	public Vector2 maneuverTime;
	public Vector2 maneuverWait;

    public GameObject PlayerShip;

	private float currentSpeed;
	private float targetManeuver;

    public GameObject Gamecontroller;

	void Start ()
	{
		currentSpeed = GetComponent<Rigidbody>().velocity.z;
		StartCoroutine(Evade());
        PlayerShip = GameObject.FindGameObjectWithTag("Player");
        Gamecontroller = GameObject.FindGameObjectWithTag("GameController");

    }

    IEnumerator Evade ()
	{
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));
		while (true)
		{
			targetManeuver = Random.Range (1, dodge) * -Mathf.Sign (transform.position.x);
			yield return new WaitForSeconds (Random.Range (maneuverTime.x, maneuverTime.y));
			targetManeuver = 0;
			yield return new WaitForSeconds (Random.Range (maneuverWait.x, maneuverWait.y));
		}
	}
	
	void FixedUpdate ()
	{
        if (!Gamecontroller.GetComponent<LightSpeed>().LighSpeedActive & Gamecontroller.GetComponent<GameController>().Player != null)
        {
            float newManeuver = Mathf.MoveTowards(GetComponent<Rigidbody>().velocity.x, targetManeuver, smoothing * Time.deltaTime);
            GetComponent<Rigidbody>().velocity = new Vector3(newManeuver, 0.0f, currentSpeed);


            GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, 0, GetComponent<Rigidbody>().velocity.x * -tilt);

            this.transform.LookAt(PlayerShip.transform);
            this.transform.Rotate(0, -180, 0);

        }
    }
}
