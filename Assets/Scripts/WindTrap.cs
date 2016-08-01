using UnityEngine;
using System.Collections;

namespace Assets.Scripts {
	public class WindTrap : MonoBehaviour {
		[SerializeField]private float _timeSinceLastChange = 0;
		[SerializeField]private GameObject _wind;
		public float TimeTurnedOn; //time until it turns off
		public float TimeTurnedOff; //time until it turns on


		// Use this for initialization
		void Start () {
			_wind = GameObject.Find ("Wind");
		}

		// Update is called once per frame
		void Update () {
			_timeSinceLastChange += Time.deltaTime;

			if(_wind.activeSelf && (_timeSinceLastChange >= TimeTurnedOn)){
				_wind.SetActive (false);
				_timeSinceLastChange = 0;
			}

			if(!_wind.activeSelf && (_timeSinceLastChange >= TimeTurnedOff)){
				_wind.SetActive (true);
				_timeSinceLastChange = 0;
			}
		}
	}
}
