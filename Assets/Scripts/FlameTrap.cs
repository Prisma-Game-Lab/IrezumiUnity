using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class FlameTrap : MonoBehaviour
    {


        GameObject playerObj;
        Player playerScript;
        Controller2D playerController;

        void Start()
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            playerScript = playerObj.GetComponent<Player>();
            playerController = playerObj.GetComponent<Controller2D>();
            
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            print("GREEN FLAME");
            if (other.gameObject.tag == "Player")
            {
                playerScript.Damage(20);
                playerController.SetPlayerWasHitAndIsInvulnerable();
            }
        }
    }
}
