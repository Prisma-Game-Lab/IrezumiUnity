using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class FlameTrap : MonoBehaviour
    {
        #region Variables
        private GameObject _playerObj;
        private Player _playerScript;
        private PlayerController _playerController;
        #endregion

        void Start()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _playerScript = _playerObj.GetComponent<Player>();
            _playerController = _playerObj.GetComponent<PlayerController>();
            
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                _playerScript.Damage(20);
                _playerController.SetPlayerWasHitAndIsInvulnerable();
            }
        }
    }
}
