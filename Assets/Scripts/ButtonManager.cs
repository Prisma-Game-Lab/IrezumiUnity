using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonManager : MonoBehaviour {

    // Singleton
    private static ButtonManager _instance;

    public static ButtonManager Instance { get { return _instance; } }

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


    /// <summary>
    /// Scene Button Callback. Load the scene passed as parameter.
    /// </summary>
    /// <param name="scene">Name if the scene.</param>
    public void SceneButtonCallback(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// 
    /// </summary>
    public void RestartButtonCallback()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevelButtonCallback()
    {
        int scene = (SceneManager.GetActiveScene().buildIndex + 1);
        if (scene > SceneManager.sceneCountInBuildSettings -1 )
        {
            scene = 2;
        }
        SceneManager.LoadScene(scene);
    }


    /// <summary>
    /// Quit Button Callback. Quits the application.
    /// </summary>
    public void QuitButtonCallback()
    {
        // Game running in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // Game running outside editor
        #else
        Application.Quit();
        #endif
    }
}
