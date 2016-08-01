using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class DoorTrigger : MonoBehaviour
    {


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                Player pscript = other.gameObject.GetComponent<Player>();
                if (pscript.IsDashing)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }

    }
}
