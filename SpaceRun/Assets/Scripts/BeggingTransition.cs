using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BeggingTransition : MonoBehaviour
{
    public GameObject Space;
    public GameObject Title;
    private GameObject[] ReferencedButtons;

    public float timeOfTravel = 5; //time for object reach a target place 
    public float currentTime = 0; // actual moving time 
    public static float PercentDone;
    public Vector3 DistanceToTravel;
    public static RectTransform rectTransform; // The transform componets
    public static Vector3 RightPosition; //Current Right position

    public void StartMove()
    {
        //set varibles=
         rectTransform = Space.GetComponent<RectTransform>(); //getting reference to this component 
         RightPosition = Space.GetComponent<RectTransform>().right;
        ReferencedButtons = this.GetComponent<ButtonActions>().buttons;

        
        //Do Stuff
        Hide();
         StartCoroutine(SpaceMove());
    }
    public void Hide()
    {
        for (int i = 0; i < ReferencedButtons.Length; i++)
        {
            ReferencedButtons[i].SetActive(false);
        }
        Title.SetActive(false);
    }

    IEnumerator SpaceMove()
    {

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            PercentDone = currentTime / timeOfTravel; // we get our percent

            rectTransform.anchoredPosition = Vector3.Lerp(RightPosition, RightPosition + DistanceToTravel, PercentDone);
            yield return null;
        }
        SceneManager.LoadScene("Main");

    }


}
