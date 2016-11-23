using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Assets.Scripts
{ 
    public class Shoot : MonoBehaviour
    {
        public GameObject projectilePrefab;

        GameObject projectile;

        [SerializeField]
        private float _timeToFirstShoot;
        [SerializeField]
        private float _cooldownTime;
        [SerializeField]
        private float _projectileSpeed;

        private bool _horDir;
        private Vector2 _oldPosition;

        private GameObject player;

        private bool playerSeen;

        public float margin;

        // Update is called once per frame
        void Start()
        {
            Physics2D.IgnoreLayerCollision(0, 15);
            Physics2D.IgnoreLayerCollision(0, 11);
            _oldPosition.x = transform.position.x;
            player = GameObject.FindGameObjectWithTag("Player");
            playerSeen = false;
            margin = 10f;
        }

        void Update()
        {
            if(!playerSeen)
            {
                if ((Mathf.Abs(transform.position.x - player.transform.position.x) < margin) && (Mathf.Abs(transform.position.y - player.transform.position.y) < margin))
                {
                    playerSeen = true;
                    StartCoroutine(spawn());
                }
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 startX = new Vector3(transform.position.x - margin, transform.position.y, transform.position.z);
            Vector3 endX = new Vector3(transform.position.x + margin, transform.position.y, transform.position.z);

            Vector3 startY = new Vector3(transform.position.x, transform.position.y - margin, transform.position.z);
            Vector3 endY = new Vector3(transform.position.x, transform.position.y + margin, transform.position.z);

            Gizmos.DrawLine(startX,endX);
            Gizmos.DrawLine(startY, endY);
        }

        IEnumerator spawn()
        {
            yield return new WaitForSeconds(_timeToFirstShoot);
            while (playerSeen)
            {
                _horDir = GetComponentInChildren<Animator>().GetBool("horDir");
                
                projectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity) as GameObject;
                projectile.gameObject.GetComponent<Projectile>().speed = _projectileSpeed;
                projectile.transform.Rotate(new Vector3(0, _horDir ? 0 : 180, 0));

                yield return new WaitForSeconds(_cooldownTime);

                _oldPosition = (Vector2)transform.position;
            }
        }
    }
}
