using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{

    public GameObject Instructionspanel;
    public GameObject Creditspanel;
    public GameObject[] buttons;
    public GameObject BackButton;

    public void ChangeScenes(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
    public void BackButtonClicked()
    {
        Instructionspanel.SetActive(false);
        Creditspanel.SetActive(false);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(true);
        }
        BackButton.SetActive(false);
    }
    public void Instructions()
    {
        if(Instructionspanel.active == false)
        {
            Instructionspanel.SetActive(true);
            for (int i = 0; i < buttons.Length ; i++)
            {
                buttons[i].SetActive(false);
            }
            BackButton.SetActive(true);

        }
        else
        {
            Instructionspanel.SetActive(false);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(true);
            }
            BackButton.SetActive(false);
        }
    }
    public void Credits()
    {
        if (Creditspanel.active == false)
        {
            Creditspanel.SetActive(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(false);
            }
            BackButton.SetActive(true);


        }
        else
        {
            Creditspanel.SetActive(false);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(true);
            }
            BackButton.SetActive(false);

        }
    }
}
