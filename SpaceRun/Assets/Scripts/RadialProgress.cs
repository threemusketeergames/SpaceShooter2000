using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{
    public Image LoadingBar;
    public GameObject circularProgressBar;
    public float speed;
    public float currentValue;

    // Update is called once per frame
    void Update()
    {
       if(currentValue <= 10)
        {
            currentValue -= speed *  Time.deltaTime;

            LoadingBar.fillAmount = currentValue / 10;
            if(currentValue <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

     public void PickUpTimer(float xcurrentValue)
    {
        if (xcurrentValue >= 10)
        {
            

            currentValue = xcurrentValue;
            
        }
        
    }
}
