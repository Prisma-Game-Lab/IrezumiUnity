using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        private GameObject _statsScreen;
        private GameObject _hpBar;
        private GameObject _pausedScreen;
        private GameObject _playerObj;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private bool _gamePaused;
        private bool _firstTime = true;
        private float _timeScaleTemp;
        private float _fixedTimeScaleTemp;
        private Player _player;
        private static GameManager _instance;
        private bool _playerIsOnScene;

        public static GameManager Instance
        {
            get { return _instance; }
        }

        #region Pause
        public bool IsPaused
        {
            get { return _gamePaused; }
        }
        
        public void EnablePause()
        {
            if (!_gamePaused)
            {
                _gamePaused = true;
                _timeScaleTemp = Time.timeScale;
                _fixedTimeScaleTemp = Time.fixedDeltaTime;
                Time.timeScale = 0;
                Time.fixedDeltaTime = 0;
            }
        }

        public void DisablePause()
        {
            if (_gamePaused)
            {
                _gamePaused = false;
                Time.timeScale = _timeScaleTemp;
                Time.fixedDeltaTime = _fixedTimeScaleTemp;
            }
        }
        #endregion

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
            OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
        }

        void OnLevelWasLoaded(int level)
        {
            DisablePause();
            if (_statsScreen = GameObject.FindGameObjectWithTag ("StatScreen")) 
            {
                _statsScreen.SetActive(false);
            }
            
            if (_pausedScreen = GameObject.FindGameObjectWithTag ("PausedScreen")) 
            {
                _pausedScreen.SetActive(false);
                _hpBar = GameObject.FindGameObjectWithTag("HPBarCanvas");	
            }
            if (_playerObj = GameObject.FindGameObjectWithTag("Player"))
            {
                _playerIsOnScene = true;
                _player = _playerObj.GetComponent<Player>();
            }
            _firstTime = true;
            _stopWatch.Reset();
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                print(SceneManager.GetActiveScene().name);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.P) && _pausedScreen)
            {
                if (!_gamePaused)
                {
                    _pausedScreen.SetActive(true);
                    print("PAUSED");
                    EnablePause();
                }
                else
                {
                    _pausedScreen.SetActive(false);
                    print("UNPAUSED");
                    DisablePause();
                }
            }
            if(_playerIsOnScene)
            {
                if (_player.FirstInput && _firstTime)
                {
                    _stopWatch.Start();
                    _firstTime = false;
                }
            }
        }

        public void LevelEnd()
        {
            StartCoroutine(SelectButtonLater());

            _statsScreen.SetActive(true);
            var stat = _statsScreen.GetComponent<StatScreen>();
            _stopWatch.Stop();
            stat.SetStats(_player, _stopWatch);
            _hpBar.SetActive(false);

            EnablePause();

            if (((int) _player.Hp) > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_Hp",0))
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Hp", ((int) _player.Hp));
            }
            if ((int) _player.InkCollected > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_Ink"))
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Ink", (int) _player.InkCollected);
            }
            if (_stopWatch.Elapsed.Minutes < PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_Minutes"))
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Minutes", _stopWatch.Elapsed.Minutes);
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Seconds", _stopWatch.Elapsed.Seconds);
            }
            else if (_stopWatch.Elapsed.Seconds < PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_Seconds"))
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Seconds", _stopWatch.Elapsed.Seconds);
            }

        }

        /// <summary>
        /// waits a frame before selecting the button.
        /// </summary>
        /// <returns></returns>
        IEnumerator SelectButtonLater()
        {
            yield return null;
            EventSystem es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            es.SetSelectedGameObject(null);
            es.SetSelectedGameObject(es.firstSelectedGameObject);
        }

    }
}
