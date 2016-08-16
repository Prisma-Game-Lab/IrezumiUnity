using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Arrow : MonoBehaviour
    {

        float speed;

        GameObject player;
        Player playerScript;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<Player>();
            speed = 20f;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
			if (other.gameObject.tag == "Player") {
				playerScript.Damage (20);
			}
			/* Revisao Pietro
            else if (other.gameObject.tag == "Trap")
            {
                Physics2D.IgnoreLayerCollision(0, 15);
            }
			Fim */
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Update()
        {
            transform.Translate(Vector2.up * Time.deltaTime * speed);
        }
    }
}
