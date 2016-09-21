using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class FallingPlatform : PlatformController
	{
        public float waitingTime;
        private float _fallingSpeed;
		private Transform _transform;
        private bool isFalling;

		// Use this for initialization
		public override void Awake ()
		{
			base.Awake ();
			_transform = GetComponent<Transform> ();

			LocalWaypoints = new Vector3[2];
			LocalWaypoints [0] = new Vector3 (0, 0, 0);
			LocalWaypoints [1] = new Vector3 (0, -10000, 0);

            isFalling = false;
			_fallingSpeed = Speed;
			Speed = 0;
		}

        IEnumerator Fall (float time)
        {
            yield return new WaitForSeconds(time);
            {
                Speed = _fallingSpeed;
                isFalling = true;
            }
        }

		void OnTriggerEnter2D (Collider2D collider)
		{
			if (collider.name == "Player" && !isFalling) {
                StartCoroutine(Fall(waitingTime));
			}
		}
	}
}

