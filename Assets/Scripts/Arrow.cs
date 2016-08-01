using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Arrow : MonoBehaviour
    {

        float speed;

        GameObject player;
        Player playerScript;

        string dir;

        public void Initialize(string direc) 
        {
            dir = direc;
        }

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<Player>();
            speed = 20f;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                playerScript.Damage(20);
            }

            if (other.gameObject.tag == "Trap")
            {
                Physics2D.IgnoreLayerCollision(0, 15);
            }

            else
            {
                Destroy(this.gameObject);
            }
        }

        void Update()
        {
            /*switch(dir)
            {
                case "up":
                    transform.Translate(Vector2.up * Time.deltaTime * speed);
                    break;
                case "down":
                    transform.Translate(Vector2.down * Time.deltaTime * speed);
                    break;
                case "left":
                    transform.Translate(Vector2.left * Time.deltaTime * speed);
                    break;
                case "right":
                    transform.Translate(Vector2.right * Time.deltaTime * speed);
                    break;
            }*/
            transform.Translate(Vector2.up * Time.deltaTime * speed);
        }
    }
}
