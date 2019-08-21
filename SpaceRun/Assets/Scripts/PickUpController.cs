using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GameController Ship;

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
    }

    public void PowerUp()
    {
        //Add to the color then set again
        CurrentColor.g += 1f;
        rend.material.color = CurrentColor;
        //Slow time down
        Time.timeScale -= 1f;
        Time.fixedDeltaTime -= 1f;
        StartCoroutine(Example());
        

    }
    IEnumerator Example()
    {
        //reset time.
        yield return new WaitForSeconds(10);
        Time.fixedDeltaTime = 2f;
        Time.timeScale = 2f;
    }
}
