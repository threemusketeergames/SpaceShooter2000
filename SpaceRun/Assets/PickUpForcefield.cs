using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpForcefield : MonoBehaviour
{
    public GameObject Ship;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Ship.GetComponent<PickUpController>().PowerUpForcefield();
            Destroy(gameObject);


        }
    }
}
