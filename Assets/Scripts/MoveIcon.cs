using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class MoveIcon : MonoBehaviour
    {

        public float Speed;

        public float Distance;

        private Vector3 _upPos;
        private Vector3 _downPos;

        private bool _touchUp;
        private bool _touchDown;

        private float _timeSpeed;

        void Start()
        {
            _touchDown = true;
            _touchUp = false;

            _upPos = new Vector3(transform.position.x, transform.position.y + Distance, transform.position.z);
            _downPos = new Vector3(transform.position.x, transform.position.y - Distance, transform.position.z);

            _timeSpeed = 0.002f;
        }

        void OnEnable()
        {
            _upPos = new Vector3(transform.position.x, transform.position.y + Distance, transform.position.z);
            _downPos = new Vector3(transform.position.x, transform.position.y - Distance, transform.position.z);
        }

        void Update()
        {
            if (!_touchUp)
            {
                transform.Translate(Vector2.up * _timeSpeed * Speed);
                if (Mathf.Abs(transform.position.y - _upPos.y) < 0.1f)
                {
                    _touchUp = true;
                    _touchDown = false;
                }
            }

            if (!_touchDown)
            {
                transform.Translate(Vector2.down * _timeSpeed * Speed);
                if (Mathf.Abs(transform.position.y - _downPos.y) < 0.1f)
                {
                    _touchUp = false;
                    _touchDown = true;
                }
            }
        }
    }
}
