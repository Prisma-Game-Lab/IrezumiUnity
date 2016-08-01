using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
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
        /// Scene Button Callback. Loads the scene passed as parameter.
        /// </summary>
        /// <param name="scene">Name if the scene.</param>
        public void SceneButtonCallback(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// Restart Button Callback. Restart the scene.
        /// </summary>
        public void RestartButtonCallback()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Next level button callback. Loads the next level.
        /// </summary>
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
}
