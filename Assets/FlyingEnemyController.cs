using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class FlyingEnemyController : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (this.tag == "DeadEnemy")
            {
                Destroy(this.gameObject);
            }

        }
    }
}
