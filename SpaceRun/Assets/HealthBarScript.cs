using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Image HealthBarImage;

    public void Start()
    {
        UpdateHealth(1,1);


    }

    public void UpdateHealth(int newhealth, int starthealth)
    {

        HealthBarImage.fillAmount = newhealth / starthealth;
    }
}