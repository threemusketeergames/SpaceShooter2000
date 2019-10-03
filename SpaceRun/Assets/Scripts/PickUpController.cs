using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpController : MonoBehaviour
{
    public GameController Ship;
    public GameObject ForceField;
    public Color CurrentColor = Color.black;
    public Renderer rend;
    public bool Ready;
    public GameObject CircularProgressBar;
    public Text PowerupDisplay;
    public bool OneLife;
    public int PowerupLastTime;

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
        //ready to start a NEW power up
        Ready = true;
        //no power up indicator
        CircularProgressBar.SetActive(false);
    }
    public void Update()
    {
        if (Ready)
        {
            PowerupDisplay.text = "Power Up:";

        }
    }

    public void DeterminPowerUp()
    {
        //var go = Instantiate(CircularProgressBar) as GameObject;
        //go.transform.parent = CircularProgressBarSpawn;
        int number = Random.Range(1, 15);

        if (Ready && !OneLife)
        {
            Ready = false;
            CircularProgressBar.SetActive(true);
            CircularProgressBar.GetComponent<RadialProgress>().PickUpTimer(10);

            if (number <= 5)
            {
                PowerUpForcefield();
            }
            else if (number > 5 && number <= 10)
            {
                PowerUpColor();
            }
            else if (number >= 11)
            {
                PowerUpBuckshot();
            }
        }
        else if(Ready && OneLife)
        {
            Ready = false;
            if (number <= 7)
            {
                //here because gaining life dosent need a bar
                CircularProgressBar.SetActive(true);
                CircularProgressBar.GetComponent<RadialProgress>().PickUpTimer(10);
                PowerUpColor();

            }
            else if (number >= 8)
            {
                PowerUpGainLife();
            }
        }
    }
            
            
        

    public void PowerUpColor()
    {
        //Add to the color then set again
        CurrentColor.g += 2.0f;
        rend.material.color = CurrentColor;
        //Slow time down
        Time.timeScale = 0.75f;
        StartCoroutine(TimeReturnColor());
        PowerupDisplay.text = "Power Up: \n Time Slow";

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
        PowerupDisplay.text = "Power Up: \n Force Field";

    }
    IEnumerator TimeReturnForcefield()
    {
        //disable forcefield.
        yield return new WaitForSeconds(10);
        ForceField.SetActive(false);
        Ready = true;
    }
    public void PowerUpBuckshot()
    {
      this.GetComponent<PlayerController>().StartBuckshot();
      PowerupDisplay.text = "Power Up:\n Buckshot";

    }
    public void PowerUpGainLife()
    {
        //gain life
        this.GetComponent<PlayerController>().gamecontrollerscript.GetComponent<GameController>().AddHealth();
        Ready = true;


    }


}
