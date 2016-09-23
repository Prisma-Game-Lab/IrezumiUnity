using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class FallingPlatform : MonoBehaviour
	{
        public float timeBeforeFall;
        public float fallingSpeed;
		private Transform _transform;
        private float time;
        private bool isFalling;
        private Vector3 movement;

		// Use this for initialization
		public void Awake ()
		{
			_transform = GetComponent<Transform> ();

            movement.x = 0;
            movement.y = 0;
            movement.z = 0;

            isFalling = false;
		}

        void Update()
        {
            if (isFalling)
            {
                time = Time.deltaTime;
                movement.y = time * (-fallingSpeed);
                _transform.Translate(movement);
            }
        }

        IEnumerator Fall (float time)
        {
            yield return new WaitForSeconds(time);
            {
                isFalling = true;
            }
        }

		void OnTriggerEnter2D (Collider2D collider)
		{
			if (collider.name == "Player" && !isFalling) {
                StartCoroutine(Fall(timeBeforeFall));
			}
		}
	}
}

