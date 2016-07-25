using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    // Singleton
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            print(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// This method is called at the end of the level.
    /// </summary>
    public void LevelEnd()
    {
        int scene = (SceneManager.GetActiveScene().buildIndex + 1);
        if ( scene + 1>SceneManager.sceneCountInBuildSettings)
        {
            scene = 0;
        }
      
        SceneManager.LoadScene(scene);
    }
}
