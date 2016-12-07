using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class LevelSelectorVertical : MonoBehaviour
    {
        private Transform[] _floorPos;

        private int _pos;

        private Vector3 _smoothVelocity;

        private Vector3 _target;

        private bool _moving;

        private Vector3 _camPos;

        public float SmoothTime;


        // Use this for initialization
        void Start()
        {
            _floorPos = new Transform[7];

            for (int i = 0; i < 7; i++)
            {
                _floorPos[i] = transform.FindChild("Floors").GetChild(i).transform;
            }
            Camera.main.transform.position = new Vector3(_floorPos[_pos].position.x, _floorPos[_pos].position.y, -10);

            SetDefault();

            ChangeCamPosition(_pos);
        }

        private void SetDefault()
        {
            _pos = 0;
            _moving = false;
            _camPos = Camera.main.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_moving)
            {
                if (InputManager.Vertical_Input() == 1)
                {
                    _pos++;
                    if (_pos > 6)
                    {
                        _pos = 6;
                    }

                    ChangeCamPosition(_pos);
                }
                else if (InputManager.Vertical_Input() == -1)
                {
                    _pos--;
                    if (_pos < 0)
                    {
                        _pos = 0;
                    }

                    ChangeCamPosition(_pos);
                }
            }

            if (InputManager.Submit_Input())
            {
                SceneManager.LoadScene(_pos);
            }

            //_camPos = Camera.main.transform.position;

            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, _target, ref _smoothVelocity, SmoothTime);

            //Camera.main.transform.position = _camPos;

            //Vector3 camPos = new Vector3(_camPos.x, _camPos.y, 0);

            if (Mathf.Abs(Camera.main.transform.position.y - _target.y) < 0.1f)
            {
                _moving = false;
            }
        }

        void ChangeCamPosition(int pos)
        {
            if (!_moving)
            {
                _target = new Vector3(_floorPos[pos].position.x, _floorPos[pos].position.y, -10);
                _moving = true;
            }
        }
    }
}
