using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{

    public GameObject panel;
    public GameObject[] buttons;

    public void ChangeScenes(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
    public void Instructions()
    {
        if(panel.active == false)
        {
            panel.SetActive(true);
            for (int i = 0; i < buttons.Length ; i++)
            {
                buttons[i].SetActive(false);
            }

        }
        else
        {
            panel.SetActive(false);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(true);
            }
        }
    }
}
