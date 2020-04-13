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
    public GameObject PickUpAudio;
    public GameObject LifeGainedAudio;


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
        //set force field
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
        float number = Random.Range(1, 15);
        //Add to the color then set again
        //PickUpAudio.GetComponent<AudioSource>().Play();
        CurrentColor.g += number;
        rend.material.color = CurrentColor;
        StartCoroutine(TimeReturnColor(number));
        PowerupDisplay.text = "Power Up: \n New Color";

    }
    IEnumerator TimeReturnColor(float Randomnumber)
    {
        //reset time.
        yield return new WaitForSeconds(10);
        CurrentColor.g -= Randomnumber;
        rend.material.color = CurrentColor;
        Ready = true;
    }

    public void PowerUpForcefield()
    {
        // PickUpAudio.GetComponent<AudioSource>().Play();
        ForceField = GameObject.FindGameObjectWithTag("ForceField");
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
        //PickUpAudio.GetComponent<AudioSource>().Play();
        this.GetComponent<PlayerController>().StartBuckshot();
      PowerupDisplay.text = "Power Up:\n Buckshot";

    }
    public void PowerUpGainLife()
    {
      //  LifeGainedAudio.GetComponent<AudioSource>().Play();
        //gain life
        this.GetComponent<PlayerController>().gamecontrollerscript.GetComponent<GameController>().AddHealth();
        Ready = true;


    }


}
