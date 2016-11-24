using UnityEngine;

namespace Assets.Scripts
{
    public class Projectile : MonoBehaviour
    {

        public float speed = 20f;

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Player>().Damage(20);
                Destroy(this.gameObject);
            }
            else if (other.gameObject.tag != "Enemy")
            {
                Destroy(this.gameObject);
            }
        }

        void Update()
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);
        }
    }
}
