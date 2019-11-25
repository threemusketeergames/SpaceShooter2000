using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class PickUpObject : MonoBehaviour
{
    public GameObject Ship;
    public AudioSource Audio;
    


    void OnTriggerEnter(Collider other)
    {

        Ship = GameObject.FindGameObjectWithTag("Player");


        if (other.tag == "Player")
        {
            Audio.Play();
            Ship.GetComponent<PickUpController>().DeterminPowerUp();
            Destroy(gameObject);


        }
    }

}

