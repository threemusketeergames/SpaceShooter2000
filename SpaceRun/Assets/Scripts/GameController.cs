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
    public Text highScore;

    public GameObject Player;
    public Transform StartPosition;

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    public Text timerAliveText;

    private bool gameOver;
    private bool restart;
    private int score;

    public int StartHeath;
    public int Health;
    public GameObject HealthBarObject;


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
        Health = StartHeath;
        Player.transform.position = StartPosition.transform.position;
        gameOver = false;
        restart = false;
        restartText.text = "";
        gameOverText.text = "";
        score = 0;
        UpdateScore();
        StartCoroutine(SpawnWaves());

        pathManager = GetComponent<PathManager>();
    }

    void Update()
    {
        if(Player != null)
        {
            timeAliveDecimal += Time.deltaTime;
            timeAlive = Mathf.RoundToInt(timeAliveDecimal);
            timerAliveText.text = "Time Alive: " + timeAlive;
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
        if (score > highscore)
        {
            highscore = score;
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

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
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
        HealthBarObject.GetComponent<HealthBarScript>().UpdateHealth(Health, StartHeath);
        if (Health <= 0)
        {
            GameOver();
            return true;
        }else
        {
            return false;
        }
    }
}