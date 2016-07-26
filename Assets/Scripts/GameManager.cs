using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

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
        int scene = (SceneManager.GetActiveScene().buildIndex + 1);
        print("pre if: " +scene);
        if (scene > SceneManager.sceneCountInBuildSettings -1 /*SceneManager.sceneCount*/)
        {
            print("no if: " + scene);
            scene = 2;
        }

        print("pos if: " + scene);
        SceneManager.LoadScene(scene);
    }
}
