using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    GameObject statsScreen;
	GameObject HpBar;

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

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
    }

    void OnLevelWasLoaded(int level)
    {
		if (statsScreen = GameObject.Find("Stat Screen"))
            statsScreen.SetActive(false);
		
		HpBar = GameObject.Find("HPBarCanvas");
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

    public void LevelEnd()
    {
        statsScreen.SetActive(true);
		HpBar.SetActive (false);
    }
}
