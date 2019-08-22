using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GameController Ship;
    public GameObject ForceField;
    public Color CurrentColor = Color.black;
    public Renderer rend;
    



    void Start()
    {
        //Get the renderer of the object so we can access the color
        rend = GetComponent<Renderer>();
        //Set the initial color (0f,0f,0f,0f)
        CurrentColor.g = 0f;
        //Set the render to the color
        rend.material.color = CurrentColor;
        //Set time scale to normal
        Time.timeScale = 2f;
        Time.fixedDeltaTime = 2f;
        //set force field to false at start
        ForceField.SetActive(false);
    }
    public void DeterminPowerUp()
    {
        int number = Random.Range(1,15);
        if(number <= 3)
        {
            PowerUpForcefield();
        }else if( number > 3 & number <= 12)
        {
            PowerUpColor();
        }else if( number >= 13)
        {
            //Buckshot power up
        }
    }

    public void PowerUpColor()
    {
        //Add to the color then set again
        CurrentColor.g += 1f;
        rend.material.color = CurrentColor;
        //Slow time down
        Time.timeScale -= 1f;
        Time.fixedDeltaTime -= 1f;
        StartCoroutine(TimeReturnColor());

    }
    IEnumerator TimeReturnColor()
    {
        //reset time.
        yield return new WaitForSeconds(10);
        Time.fixedDeltaTime = 2f;
        Time.timeScale = 2f;
    }

    public void PowerUpForcefield()
    {
        ForceField.SetActive(true);
        StartCoroutine(TimeReturnForcefield());
        //undestroyable
    }
    IEnumerator TimeReturnForcefield()
    {
        //reset time.
        yield return new WaitForSeconds(15);
        ForceField.SetActive(false);
    }
}
