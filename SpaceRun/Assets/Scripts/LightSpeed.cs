using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpeed : MonoBehaviour
{
    public ParticleSystemRenderer Stars;
    public GameObject Ship;

    public void Start()
    {
        Stars.lengthScale = 3;
        Ship.GetComponent<ShipMovement>().moveSpeed= 0.0f;
        StartCoroutine(WaitFor());

    }

    public void Update()
    {
        //Stars.lengthScale += 5;
    }

    IEnumerator WaitFor()
    {
        //while (Stars.lengthScale < 190)
        //{
        //    Stars.lengthScale -= 50;
        //    yield return new WaitForSeconds(2);

        //}
        //while (Stars.lengthScale > 50)
        //{
        //    Stars.lengthScale -= 50;
        //    yield return new WaitForSeconds(2);

        //}
        //Stars.lengthScale = 3;
        //Ship.GetComponent<ShipMovement>().moveSpeed = 4f;
        float elapsed = 0;
        float duration = 20;

        while (elapsed < duration)
        {
            Stars.lengthScale = Mathf.Lerp(0, 200, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
    
        

    }
}
