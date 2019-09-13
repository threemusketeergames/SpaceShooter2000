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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       if(currentValue <= 100)
        {
            currentValue -= speed * Time.deltaTime;

            LoadingBar.fillAmount = currentValue / 100;
            if(currentValue <= 0)
            {
                Destroy(circularProgressBar);
            }
        }
    }

     public void PickUpTimer(float xcurrentValue)
    {
        if (xcurrentValue >= 100)
        {
            

            currentValue = xcurrentValue;
            
        }
        
    }
}
