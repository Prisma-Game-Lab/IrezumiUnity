using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Spike : MonoBehaviour
    {
        void Start()
        { 

        }
        #region CollisionEnter
        void OnCollisionEnter2D(Collision2D other)
        {
           
            if (other.gameObject.tag == "Player")
            {
               // other.gameObject.SendMessage("Damage", 1000);
               other.gameObject.GetComponent<Player>().Damage(1000);
            }
            /* Revisao Pietro
            else if (other.gameObject.tag == "Trap")
            {
                Physics2D.IgnoreLayerCollision(0, 15);
            }
            Fim */
        }

        #endregion

        void Update()
        {

        }
    }
}