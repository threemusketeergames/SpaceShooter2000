using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int score = Random.Range(1, 1000);
            string unsername = "";
            string alfabet = "abcdefghijklmnopqrstuvwxyz";
            for(int i = 0; i <Random.Range(2,9); i++)
            {
                unsername += alfabet[Random.Range(0, alfabet.Length)];

            }
            Highscores.AddNewHighscore(unsername, score);
        }
    }
}
