using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Highscores : MonoBehaviour
{

    const string privateCode = "h7O7xT3wqU-utfeBviQ0lA1SjHS6xfWU-n_Abw3UzkRQ";
    const string publicCode = "5e8b6dac403c2d12b8c085e8";
    const string webURL = "http://dreamlo.com/lb/";

    DisplayHighScores highscoreDisplay;
    public Highscore[] highscoresList;
    static Highscores instance;
    public string playerhighscore;
    public GameObject PlayerdataO;
    public Text HighScoreTextO;

    void Awake()
    {
        highscoreDisplay = GetComponent<DisplayHighScores>();
        instance = this;
        PlayerdataO = GameObject.FindGameObjectWithTag("Playerdata");
        GetSingleScore(PlayerdataO.GetComponent<PlayerData>().PlayerName);

    }

    public static void AddNewHighscore(string username, int score)
    {
        instance.StartCoroutine(instance.UploadNewHighscore(username, score));
    }

    IEnumerator UploadNewHighscore(string username, int score)
    {
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Upload Successful");
            DownloadHighscores();
        }
        else
        {
            print("Error uploading: " + www.error);
        }
    }

    public void DownloadHighscores()
    {
        StartCoroutine("DownloadHighscoresFromDatabase");
    }

    IEnumerator DownloadHighscoresFromDatabase()
    {
        WWW www = new WWW(webURL + publicCode + "/pipe/");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            FormatHighscores(www.text);
            highscoreDisplay.OnHighscoresDownloaded(highscoresList);
        }
        else
        {
            print("Error Downloading: " + www.error);
        }
    }

    void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoresList = new Highscore[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoresList[i] = new Highscore(username, score);
            print(highscoresList[i].username + ": " + highscoresList[i].score);
        }
    }

    public void GetSingleScore(string playerName)
    {
        playerhighscore = "";
        if(playerName == "")
        {
            HighScoreTextO.text = "Highscore: ";
        }
        else
        {
            StartCoroutine(GetRequest(webURL + publicCode + "/pipe-get/" + UnityWebRequest.EscapeURL(playerName)));
        }
    }

    IEnumerator GetRequest(string url)
    {
        // Something not working? Try copying/pasting the url into your web browser and see if it works.
        // Debug.Log(url);

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            playerhighscore = www.downloadHandler.text;
            string[] searchItems = playerhighscore.Split('|');
            string result = searchItems[1].Trim();
            HighScoreTextO.text = "HighScore: " + result;
        }

        

    }
}
    public struct Highscore
{
    public string username;
    public int score;

    public Highscore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }

}