using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class GooTrap : MonoBehaviour
    {
        Player pscript;
        bool isOnTrap;

        void DeactivateTrap()
        {
            isOnTrap = false;
            pscript.MoveSpeed = pscript.MoveSpeed * 2;
            
        }

        void OnTriggerEnter2D(Collider2D other)
        {

            if (other.gameObject.tag == "Player" && isOnTrap == false)
            {
                isOnTrap = true;
                pscript = other.gameObject.GetComponent<Player>();
                Invoke("DeactivateTrap", 1.5f);
                pscript.MoveSpeed = pscript.MoveSpeed / 2;

            }
        }

    }
}

