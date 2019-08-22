using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject Ship;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Ship.GetComponent<PickUpController>().DeterminPowerUp();
            Destroy(gameObject);


        }
    }

}

