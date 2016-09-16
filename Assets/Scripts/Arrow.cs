using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Arrow : MonoBehaviour
    {
        private float speed;

        void Start()
        {
            speed = 20f;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
			if (other.gameObject.tag == "Player") {
				other.gameObject.GetComponent<Player>().Damage (20);
				Destroy (this.gameObject);
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
