using UnityEngine;

namespace Assets.Scripts
{
    public class LevelEnd : MonoBehaviour {

        private GameManager _gameManager;

        void Start()
        {
            _gameManager = GameManager.Instance;
        }

        void OnTriggerEnter2D (Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                _gameManager.LevelEnd();
            }
        }
    }
}
