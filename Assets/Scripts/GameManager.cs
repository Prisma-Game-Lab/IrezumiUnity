using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{

    GameObject statsScreen;
    GameObject HpBar;

    GameObject pausedScreen;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get { return _instance; }
    }

    private bool _gamePaused = false;

    private float _timeScaleTemp;

    private void TogglePause()
    {
        if (!_gamePaused)
        {
            _gamePaused = true;
            _timeScaleTemp = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            _gamePaused = false;
            Time.timeScale = _timeScaleTemp;
        }
    }

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

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
    }

    void OnLevelWasLoaded(int level)
    {
		if (statsScreen = GameObject.FindGameObjectWithTag ("StatScreen")) 
		{
			statsScreen.SetActive(false);
		}
            
		if (pausedScreen = GameObject.FindGameObjectWithTag ("PausedScreen")) 
		{
			pausedScreen.SetActive(false);
			HpBar = GameObject.FindGameObjectWithTag("HPBarCanvas");	
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

        if (Input.GetKeyDown(KeyCode.P) && pausedScreen)
        {
            if (!_gamePaused)
            {
                pausedScreen.SetActive(true);
                print("PAUSED");
            }
            else
            {
                pausedScreen.SetActive(false);
                print("UNPAUSED");
            }
            TogglePause();
        }
    }

    public void LevelEnd()
    {
        statsScreen.SetActive(true);
		HpBar.SetActive (false);
    }

    public bool IsPaused()
    {
        return _gamePaused;
    }
}
