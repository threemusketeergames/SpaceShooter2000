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

    public GameObject Player;

    public Text restartText;
    public Text gameOverText;
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
        gameOverText.text = "";
        StartCoroutine(SpawnWaves());

        pathManager = GetComponent<PathManager>();
    }


    void Update()
    {
        TimeScaleIndicator = Time.timeScale;
        if(Player != null && !this.GetComponent<LightSpeed>().LighSpeedActive)
        {
            timeAliveDecimal += Time.unscaledDeltaTime; //not affected by game speeding up. 
            timeAlive = Mathf.RoundToInt(timeAliveDecimal) -timebouns;
            timerAliveText.text = "Time Alive: " + timeAlive;
        }

        if (timeAlive > 10)
        {
            GameFaster();
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
                //ahead of the player
                Vector3 spawnPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, (Player.transform.position.z + DistanceFromPlayer));
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
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
        
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        gameOver = true;
        PlayerHighScored();
    }
    public bool TakeHealth()
    {
        Health -= 50;
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
    public void GameFaster()
    {
        if(timeAlive % 5 == 0 && TimeSpeedReady)
        {
            TimeSpeedReady = false;
            WaitSeconds(2);
            TimeSpeedReady = true;
        }
        
    }
    IEnumerator WaitSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);


    }
}