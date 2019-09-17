using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpeed : MonoBehaviour
{
    public ParticleSystemRenderer Stars;
    public GameObject Ship;

    public void Start()
    {
        Stars.lengthScale = 3f;
        Ship.GetComponent<ShipMovement>().moveSpeed= 0.0f;
        CommenceLightSpeed();

    }

    public void CommenceLightSpeed()
    {
        while(Stars.lengthScale < 190f)
        {
            Stars.lengthScale += 30f * Time.deltaTime;
        }
        while(Stars.lengthScale > 50f)
        {
            Stars.lengthScale -= 50f * Time.deltaTime;
        }
        Stars.lengthScale = 3f;
        Ship.GetComponent<ShipMovement>().moveSpeed = 4.0f;

    }
}
