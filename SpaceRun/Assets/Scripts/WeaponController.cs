using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float delay;
    public GameObject Gamecontroller;
	void Start ()
	{
        Gamecontroller = GameObject.FindGameObjectWithTag("GameController");
        InvokeRepeating ("Fire", delay, fireRate);
	}

	void Fire ()
	{
        if (!Gamecontroller.GetComponent<LightSpeed>().LighSpeedActive)
        {
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            GetComponent<AudioSource>().Play();
        }

	}
}
