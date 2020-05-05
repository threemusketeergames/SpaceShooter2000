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
    public bool Forcefieldinitiated;


    //public Transform CircularProgressBarSpawn;

    void Start()
    {
        rend = GetComponent<Renderer>();
        CurrentColor.g = 0f;
        rend.material.color = CurrentColor;
        Time.timeScale = 1f;
        ForceField.SetActive(false);
        Ready = true;
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

            if (number <= 6)
            {
                PowerUpForcefield();
            }
            else if (number > 6 && number <= 9)
            {
                PowerUpColor();
            }
            else if (number >= 10)
            {
                PowerUpBuckshot();
            }
        }
        else if(Ready && OneLife)
        {
            Ready = false;
            if (number <= 7)
            {
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
        PickUpAudio.GetComponent<AudioSource>().Play();
        CurrentColor.g += number;
        rend.material.color = CurrentColor;
        StartCoroutine(TimeReturnColor(number));
        PowerupDisplay.text = "Power Up: \n New Color";

    }
    IEnumerator TimeReturnColor(float Randomnumber)
    {
        yield return new WaitForSeconds(10);
        CurrentColor.g -= Randomnumber;
        rend.material.color = CurrentColor;
        Ready = true;
    }

    public void PowerUpForcefield()
    {
        PickUpAudio.GetComponent<AudioSource>().Play();
        Forcefieldinitiated = true;
        ForceField.SetActive(true);
        StartCoroutine(TimeReturnForcefield());
        PowerupDisplay.text = "Power Up: \n Force Field";

    }
    IEnumerator TimeReturnForcefield()
    {
        yield return new WaitForSeconds(10);
        ForceField.SetActive(false);
        Forcefieldinitiated = false;
        Ready = true;
    }
    public void PowerUpBuckshot()
    {
        PickUpAudio.GetComponent<AudioSource>().Play();
        this.GetComponent<PlayerController>().StartBuckshot();
      PowerupDisplay.text = "Power Up:\n Buckshot";

    }
    public void PowerUpGainLife()
    {
        LifeGainedAudio.GetComponent<AudioSource>().Play();
        this.GetComponent<PlayerController>().gamecontrollerscript.GetComponent<GameController>().AddHealth();
        Ready = true;


    }


}
