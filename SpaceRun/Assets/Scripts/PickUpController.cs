﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GameController Ship;
    public GameObject ForceField;
    public Color CurrentColor = Color.black;
    public Renderer rend;
    public bool Ready;
    public GameObject CircularProgressBar;
    //public Transform CircularProgressBarSpawn;

    void Start()
    {
        //Get the renderer of the object so we can access the color
        rend = GetComponent<Renderer>();
        //Set the initial color (0f,0f,0f,0f)
        CurrentColor.g = 0f;
        //Set the render to the color
        rend.material.color = CurrentColor;
        //Set time scale to normal
        Time.timeScale = 1f;
        //set force field to false at start
        ForceField.SetActive(false);
        //ready to start
        Ready = true;
    }
    public void DeterminPowerUp()   
    {
        //var go = Instantiate(CircularProgressBar) as GameObject;
        //go.transform.parent = CircularProgressBarSpawn;
        if (Ready)
        {
            Ready = false;
            CircularProgressBar.active = true;
            CircularProgressBar.GetComponent<RadialProgress>().PickUpTimer(100);
            int number = Random.Range(1, 15);
            if (number <= 3)
            {
                PowerUpForcefield();
            }
            else if (number > 3 & number <= 12)
            {
                PowerUpColor();
            }
            else if (number >= 13)
            {
                PowerUpBuckshot();
            }
        }
        
    }

    public void PowerUpColor()
    {
        //Add to the color then set again
        CurrentColor.g += 1f;
        rend.material.color = CurrentColor;
        //Slow time down
        Time.timeScale = 0.5f;
        StartCoroutine(TimeReturnColor());

    }
    IEnumerator TimeReturnColor()
    {
        //reset time.
        yield return new WaitForSeconds(10);
        Time.timeScale = 1f;
        Ready = true;
    }

    public void PowerUpForcefield()
    {
        ForceField.SetActive(true);
        StartCoroutine(TimeReturnForcefield());
        //undestroyable
    }
    IEnumerator TimeReturnForcefield()
    {
        //disable forcefield.
        yield return new WaitForSeconds(15);
        ForceField.SetActive(false);
        Ready = true;
    }
    public void PowerUpBuckshot()
    {
      this.GetComponent<PlayerController>().StartBuckshot();
    }
}
