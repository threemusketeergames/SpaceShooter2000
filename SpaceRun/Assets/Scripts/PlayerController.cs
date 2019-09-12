using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public class Boundary
//{
//    public float xMin, xMax, zMin, zMax;
//}
public class PlayerController : MonoBehaviour
{
    //public float speed;
    //public float tilt;
    //public Boundary boundary;

    public GameObject bullet;
    public Transform shotSpawn;
    public Transform shotSpawnBuckshot1;
    public Transform shotSpawnBuckshot2;
    public GameObject Buckshotbullet;
    public bool buckshoton;
    public float fireRate;

    private float nextFire;

    void Update()
    {

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            if (buckshoton)
            {
                nextFire = Time.time + fireRate;
                Instantiate(Buckshotbullet, shotSpawn.position, shotSpawn.rotation);
                Instantiate(Buckshotbullet, shotSpawnBuckshot1.position, shotSpawn.rotation);
                Instantiate(Buckshotbullet, shotSpawnBuckshot2.position, shotSpawn.rotation);
                GetComponent<AudioSource>().Play();
            }
            else
            {
                nextFire = Time.time + fireRate;
                Instantiate(bullet, shotSpawn.position, shotSpawn.rotation);
                GetComponent<AudioSource>().Play();
            }
          
        }
    }
    public void StartBuckshot()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        buckshoton = true;
        yield return new WaitForSeconds(10);
        buckshoton = false;
        this.GetComponent<PickUpController>().Ready = true;


    }



    void FixedUpdate()
    {
        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");

        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        //GetComponent<Rigidbody>().velocity = movement * speed;

        //GetComponent<Rigidbody>().position = new Vector3
        //(
        //    Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
        //    0.0f,
        //    Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
        //);

        //GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
    }
}
