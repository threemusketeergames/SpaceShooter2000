using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
	public float speed;
    public int destroyTime = 1;
	void Update()
	{
		//GetComponent<Rigidbody>().velocity = transform.forward * speed;
        transform.Translate(Vector3.back * speed);
        Destroy(this.gameObject, destroyTime);
    }
}
