using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerGraphicsScript : MonoBehaviour {

        // Use this for initializatio
        private Player _playerScript;

        void Start ()
        {
            _playerScript = GameObject.Find("Player").GetComponent<Player>();

        }

        public void StartDash()
        {
            _playerScript.IsDashing = true;
            //Invoke("ResetDashing", 0.3666666f);
        }

        public void StopDashing()
        {
            _playerScript.ResetAllDashing();

        }

        // Update is called once per frame
        void Update () {
	
        }
    }
}
