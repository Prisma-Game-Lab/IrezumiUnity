using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
	public class FallingPlatform : MonoBehaviour
	{
        //public float timeBeforeFall; //trocado por timeBeforeShake
        public float fallingSpeed;
		private Transform _transform;
        private float time;
        private bool isFalling;
		private bool _wasTouched;
        private Vector3 movement;

		////////////////////////////////////
		// Speed-Damp related variables
		////////////////////////////////////
		/// <summary>
		/// How long it takes for the speed to reach its maximum
		/// </summary>
		public float fallingAccelerationTime = 0.5f;
		private float _actualSpeed = 0f;
		private float _speedDT;
	
		////////////////////////////////////
		// Shake related variables
		////////////////////////////////////
		public float timeBeforeShake = 2;
		public float shakeDuration = 0.6f;
		/// <summary>
		/// How much the object will move to the sides, in points
		/// </summary>
		public float shakeMovementIntensity = 0.27f;
		/// <summary>
		/// How much the object will rotate, in degrees (angle)
		/// </summary>
		public float shakeRotationIntensity = 11;
		public int numberOfShakes = 5;


		public void Awake ()
		{
			_transform = GetComponent<Transform> ();

            movement.x = 0;
            movement.y = 0;
            movement.z = 0;

            isFalling = false;
			_wasTouched = false;
		}

        void Update()
		{
            if (isFalling)
            {
				_actualSpeed = Mathf.SmoothDamp (_actualSpeed, fallingSpeed, ref _speedDT, fallingAccelerationTime); //Makes the speed goes from 0 to fallSpeed over time
                time = Time.deltaTime;
				movement.y = time * (-_actualSpeed);
                _transform.Translate(movement);
            }
        }

		// Creates a gameObject with the same sprite and as a child of this.
		private GameObject CreateChildGraphicCopy(){
			GameObject go = new GameObject ();
			go.AddComponent<SpriteRenderer> ().sprite = GetComponent<SpriteRenderer> ().sprite;
			go.transform.parent = transform;
			go.transform.localScale = Vector3.one;
			return go;
		}

		IEnumerator Action(){
			yield return new WaitForSeconds (timeBeforeShake); //Wait for shake
			float eachShakeDur = shakeDuration/numberOfShakes; //Calculate time between shakes
			Vector3 pos = transform.localPosition;
			GameObject copy = CreateChildGraphicCopy (); //Gets a copy of the sprite
			GetComponent<SpriteRenderer> ().enabled = false; //Disable this sprite
			float m = shakeMovementIntensity, r = shakeRotationIntensity;
			for (int i = 0; i < numberOfShakes; i++) {
				// Perform shake
				copy.transform.localRotation = Quaternion.Euler(0f,0f,Random.Range(r,-r)); //Rotates the copy
				copy.transform.localPosition = new Vector3(Random.Range(-m,m),Random.Range(-m,m)); //Moves the copy
				yield return new WaitForSeconds (eachShakeDur);
			}
			GetComponent<SpriteRenderer> ().enabled = true; //Re-enable this sprite
			Destroy (copy); //Remove the copy
			isFalling = true; //Enable falling
		}

        IEnumerator Fall (float time)
        {
            yield return new WaitForSeconds(time);
            {
                isFalling = true;
            }
        }

		// Integration betweek Fall and TriggerEnter
		// wasn't avoiding multiple calls, since isFalling
		// is set only after some time.
		// Another boolean (wasTouched) avoids unnecessary
		// additional calls to the coroutine

		void OnTriggerEnter2D (Collider2D collider)
		{
			if (collider.name == "Player" && !_wasTouched) {
				_wasTouched = true;
				StartCoroutine (Action());
				//StartCoroutine(Fall(timeBeforeFall));
			}
		}
	}
}

