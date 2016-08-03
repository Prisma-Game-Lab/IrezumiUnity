using UnityEngine;
using System.Collections;

namespace Assets.Scripts{
	public class Wind : MonoBehaviour {
		private GameObject playerObj;
		private Player player;
		private bool _forcePushActivated;
		private float CurrentSpeed;
		private float DampSpeed = 4;
		public float WindThrowSpeed;

		// Use this for initialization
		void Start () {
			playerObj = GameObject.Find ("Player");
			player = playerObj.GetComponent<Player> ();
			_forcePushActivated = false;
			CurrentSpeed = 0;
		}

		// Update is called once per frame
		void Update () {
			
			if(_forcePushActivated){
				CurrentSpeed = Mathf.SmoothDamp (CurrentSpeed, WindThrowSpeed, ref DampSpeed, 0.5f);
				player.ForcePush(new Vector2(CurrentSpeed, -10), false);

				if(CurrentSpeed > WindThrowSpeed - 1){
					_forcePushActivated = false;
					CurrentSpeed = 0;
				}
			}

		}

		/// <summary>
		/// Raises the collision enter 2d event.
		/// </summary>
		/// <param name="collider">Collider.</param>
		void OnCollisionEnter2D(Collision2D collider){
			if (collider.collider.gameObject.Equals (playerObj)) {
				_forcePushActivated = true;
			}
		}
	}
}
