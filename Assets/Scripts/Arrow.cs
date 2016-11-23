using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Arrow : MonoBehaviour
    {
        public float speed = 20f;

        void Start()
        {
            //speed = 20f;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
			if (other.gameObject.tag == "Player") {
				other.gameObject.GetComponent<Player>().Damage (20);
				Destroy (this.gameObject);
			}
		   // Revisao Pietro
            else if (other.gameObject.tag == "Enemy"|| other.gameObject.tag == "FlyingEnemy")
            {
                Physics2D.IgnoreCollision(other.gameObject.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(),true);
                //Physics2D.IgnoreLayerCollision(0, 18);
            }
			
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
