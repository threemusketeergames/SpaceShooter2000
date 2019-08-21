using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpGenerator : MonoBehaviour
{
    public GameObject Ship;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Ship.GetComponent<PickUpController>().PowerUp();
            Destroy(gameObject);


        }
    }

}

