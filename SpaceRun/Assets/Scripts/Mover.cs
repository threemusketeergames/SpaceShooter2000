using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
	public float speed;

	void Update()
	{
		//GetComponent<Rigidbody>().velocity = transform.forward * speed;
        transform.Translate(Vector3.back * speed);

    }
}
