using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;
	private GameController gameController;
    public Transform Camera;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}

	void OnTriggerEnter (Collider other)
	{
  
		if (other.tag == "Boundary" || other.tag == "Enemy" || other.tag == "ForceField")
		{
			return;
		}

		if (explosion != null)
		{
			Instantiate(explosion, transform.position, transform.rotation);

        }
        if (other.tag == "Bullet")
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
            gameController.SubtractTime(scoreValue);
        }

        if (other.tag == "Player")
		{
            //demote the health in gamecontroller
            if (gameController.TakeHealth(50))
            {
                //remove parent before destroying ship
                Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
                Camera.parent = null;
                Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                Destroy(other.gameObject);
                Destroy(gameObject);
                StartCoroutine(TimeStop());
            }
            else
            {
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
           
		}


	}
    IEnumerator TimeStop()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0.0f;

    }
}