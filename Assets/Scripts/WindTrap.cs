using UnityEngine;
using System.Collections;

namespace Assets.Scripts {
	public class WindTrap : MonoBehaviour
    {
        private static GameObject[] _windsArray;
        private static int _windIndex;
        private float _timeSinceLastChange = 0;
		private GameObject _wind;
		public float TimeTurnedOn; //time until it turns off
		public float TimeTurnedOff; //time until it turns on


		// Use this for initialization
		void Start () {
            _windsArray = GameObject.FindGameObjectsWithTag("Wind");

            foreach(GameObject obj in _windsArray){
                Debug.Log(obj.name);
            }

			_wind = InitializeWind();
		}

        GameObject InitializeWind()
        {
            _windIndex++;
            if(_windIndex >= _windsArray.Length)
            {
                _windIndex -= _windsArray.Length;
            }

            return _windsArray[_windIndex];
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
