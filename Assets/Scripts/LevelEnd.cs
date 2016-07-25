using UnityEngine;
using System.Collections;

public class LevelEnd : MonoBehaviour {

    GameObject gameManager;

    GameManager gm;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager");
        gm = gameManager.GetComponent<GameManager>();
    }

    /// <summary>
    /// Method called on trigger enter.
    /// </summary>
    /// <param name="other">Collider responsible for triggering the trigger</param>
	void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            gm.LevelEnd();
        }
    }
}
