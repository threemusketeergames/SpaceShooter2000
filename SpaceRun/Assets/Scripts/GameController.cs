using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject[] hazards;
    public float startWait;
    public int hazardCount;
    public int DistanceFromPlayer;
    public float spawnWait;
    public float waveWait;
    public float timeAliveDecimal;
    public int timeAlive;
    static int highscore = 0;
    public int timebouns;
    public Text highScore;
    public GameObject PlayerData;

    public GameObject Player;

    public Text restartText;
    public GameObject Leaderboard;
    public Text timerAliveText;

    private bool gameOver;
    private bool restart;

    public int StartHeath;
    public int Health;
    public Text LivesDisplay;
    public bool Canshoot;
    public GameObject EngineParticle;
    public float TimeScaleIndicator;
    public bool TimeSpeedReady;


    public PathManager pathManager;
    public GameObject playerExplosion;
    private Transform Camera;

    private void Awake()
    {
        highScore.text = "High Score: " + highscore.ToString();


        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        Time.timeScale = 1.0f;
        Canshoot = true;
        Health = StartHeath;
        LivesDisplay.text = "Lives:" + "X X";
        //Player.transform.position = StartPosition.transform.position;
        gameOver = false;
        restart = false;
        restartText.text = "";
        Leaderboard.gameObject.SetActive(false);
        StartCoroutine(SpawnWaves());
        TimeSpeedReady = true;
        pathManager = GetComponent<PathManager>();
        PlayerData = GameObject.FindGameObjectWithTag("Playerdata");
    }


    void Update()
    {
        TimeScaleIndicator = Time.timeScale;
        if(Player != null && !this.GetComponent<LightSpeed>().LighSpeedActive && !gameOver)
        {
            timeAliveDecimal += Time.unscaledDeltaTime; //not affected by game speeding up. 
            timeAlive = Mathf.RoundToInt(timeAliveDecimal) + timebouns;
            timerAliveText.text = "Time Alive: " + timeAlive;
        }

        if(  timeAliveDecimal < 25  & timeAliveDecimal > 5 & TimeSpeedReady)
        {
            TimeSpeedReady = false;
            Time.timeScale += 0.005f;
            StartCoroutine(WaitSecondsTimeSpeed(2));

        }
        else if( timeAliveDecimal > 25 & TimeSpeedReady)
        {
            TimeSpeedReady = false;
            Time.timeScale += 0.10f;
            StartCoroutine(WaitSecondsTimeSpeed(3));
        }

        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
    public void PlayerHighScored()
    {
        if (timeAlive > highscore)
        {
            highscore = timeAlive;
            highScore.text = "High Score: " + highscore.ToString();
            Highscores.AddNewHighscore(PlayerData.GetComponent<PlayerData>().PlayerName, highscore);
        }

    }
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                // Spawin where??? ...rn..ahead of the player
                Vector3 spawnPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, (Player.transform.position.z + DistanceFromPlayer));
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
                //
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);

            if (gameOver)
            {
                restartText.text = "Press 'R' for Restart";
                restart = true;
                break;
            }
        }
    }

    public void SubtractTime(int newScoreValue)
    {
        timebouns += newScoreValue;
        timerAliveText.color = Color.green;
        StartCoroutine(WaitForSecondsTimeColor(0.5f));


    }

    public void GameOver()
    {
        Leaderboard.gameObject.SetActive(true);
        Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Instantiate(playerExplosion, Player.transform.position, Player.transform.rotation);
        Camera.parent = null;
        Destroy(Player);
        gameOver = true;
        PlayerHighScored();

    }
    public bool TakeHealth(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            //lost our second life
            Canshoot = true;
            EngineParticle.SetActive(true);
            LivesDisplay.text = "Lives:" + "";
            GameOver();
            return true;
        }else
        {
            //lost first life
            Canshoot = false;
            EngineParticle.SetActive(false);
            LivesDisplay.text = "Lives:" + "X";
            Player.GetComponent<PickUpController>().OneLife = true;
            return false;
        }
    }
    public void AddHealth()
    {
        Health += 50;
        LivesDisplay.text = "Lives:" + "XX";
        Canshoot = true;
        EngineParticle.SetActive(true);
        Player.GetComponent<PickUpController>().OneLife = false;

    }

    IEnumerator WaitSecondsTimeSpeed(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        TimeSpeedReady = true;

    }
    IEnumerator WaitForSecondsTimeColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        timerAliveText.color = Color.white;
            

    }
}