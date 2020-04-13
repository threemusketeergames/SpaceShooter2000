using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    public GameObject bullet;
    public Transform shotSpawn;
    public Transform shotSpawnBuckshot1;
    public Transform shotSpawnBuckshot2;
    public GameObject Buckshotbullet;
    public bool buckshoton;
    public float fireRate;
    private float firelimit;
    public bool Recharging;
    public GameObject ProgressBar;
    public int RechargeTime;
    public bool PlayerInBounds;



    private float nextFire;
    public GameObject gamecontrollerscript;
    private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

    private void Start()
    {
        firelimit = 15;
        Recharging = false;
        PlayerInBounds = true;
       // Physics.IgnoreLayerCollision(0, 8);
    }

    void Update()
    {

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            if (gamecontrollerscript.GetComponent<GameController>().Canshoot && !gamecontrollerscript.GetComponent<LightSpeed>().LighSpeedActive)
            {
                if(firelimit != 0)
                {
                    firelimit -= 1;
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


        }
        if(firelimit == 0 & !Recharging)
        {
            Recharging = true;
            ProgressBar.GetComponent<RadialProgress>().PickUpTimer(RechargeTime);
            StartCoroutine(ResetFireLimit());
        }
        
    }
    IEnumerator ResetFireLimit()
    {
        yield return new WaitForSeconds(RechargeTime);
        firelimit = 15;
        Recharging = false;
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
    void OnTriggerStay(Collider Other)
    {
        if (Other.gameObject.tag == "Tube")
        {
           // Physics.IgnoreCollision(this.GetComponent<Collider>(), Other);
            PlayerInBounds = true;
        }
    }

    void OnTriggerExit(Collider Other)
    {
        if (Other.gameObject.tag == "Tube")
        {
           // Physics.IgnoreCollision(this.GetComponent<Collider>(), Other);
            PlayerInBounds = false;
            StartCoroutine(Determinbounds());
        }
    }
    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.tag == "Tube")
        {
            //Physics.IgnoreCollision(this.GetComponent<Collider>(), Other);
            PlayerInBounds = true;
        }
    }
    IEnumerator Determinbounds()
    {
        yield return new WaitForSeconds(0.1f);
        if (!PlayerInBounds)
        {
            gamecontrollerscript.GetComponent<GameController>().TakeHealth(100);
        }
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
