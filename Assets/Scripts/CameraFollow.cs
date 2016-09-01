using UnityEngine;

namespace Assets.Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        #region Variables
        public Controller2D Target;
        public float VerticalOffset;
        public float LookAheadDstX;
        public float LookSmoothTimeX;
        public float VerticalSmoothTime;
        public Vector2 FocusAreaSize;
        public bool CamShake = false;
        public float _shakeDuration;
        public float _shakeAmount;

        private FocusArea _focusArea;
        private float _currentLookAheadX;
        private float _targetLookAheadX;
        private float _lookAheadDirX;
        private float _smoothLookVelocityX;
        private float _smoothLookVelocityY;
        private float _smoothVelocityY;
        private bool _lookAheadStopped;

        
        #endregion

        #region Start
        public void Start()
        {
          //  SetDefaut();
            _focusArea = new FocusArea(Target.Collider.bounds, FocusAreaSize);
        }

        private void SetDefaut()
        {
            FocusAreaSize = new Vector2(3, 5);
            VerticalOffset = 0;
            LookAheadDstX = 5;
            LookSmoothTimeX = .25f;
            VerticalSmoothTime = .25f;
            _shakeDuration = 0.5f;
            _shakeAmount = 1;
        }
        #endregion

        #region Draw
        public void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawCube(_focusArea.Centre, FocusAreaSize);
        }
        #endregion

        #region Update
        public void LateUpdate()
        {
            _focusArea.Update(Target.Collider.bounds);

            var focusPosition = _focusArea.Centre + Vector2.up * VerticalOffset;

            SetTargetLookAheadX();

            _currentLookAheadX = Mathf.SmoothDamp(_currentLookAheadX, _targetLookAheadX, ref _smoothLookVelocityX, LookSmoothTimeX);

            focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref _smoothVelocityY, VerticalSmoothTime);
            focusPosition += Vector2.right * _currentLookAheadX;

            if (CamShake)
            {
                float randomValue = Random.Range(-Mathf.PI, Mathf.PI);
                focusPosition.x = Mathf.SmoothDamp(focusPosition.x, focusPosition.x + Mathf.Cos(randomValue) * _shakeAmount, ref _smoothLookVelocityX, 0.02f);
                focusPosition.y = Mathf.SmoothDamp(focusPosition.y, focusPosition.y + Mathf.Sin(randomValue) * _shakeAmount, ref _smoothLookVelocityY, 0.02f);
            }
            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
        }

        private void SetTargetLookAheadX()
        {
            if (_focusArea.Velocity.x != 0)
            {
                _lookAheadDirX = Mathf.Sign(_focusArea.Velocity.x);
                if (Mathf.Sign(Target.PlayerInput.x) == Mathf.Sign(_focusArea.Velocity.x) && Target.PlayerInput.x != 0)
                {
                    _lookAheadStopped = false;
                    _targetLookAheadX = _lookAheadDirX*LookAheadDstX;
                }
                else
                {
                    if (!_lookAheadStopped)
                    {
                        _lookAheadStopped = true;
                        _targetLookAheadX = _currentLookAheadX + (_lookAheadDirX*LookAheadDstX - _currentLookAheadX)/4f;
                    }
                }
            }
        }
        #endregion

        #region Structs
        private struct FocusArea
        {
            public Vector2 Centre;
            public Vector2 Velocity;
            private float _left, _right;
            private float _top, _bottom;


            public FocusArea(Bounds targetBounds, Vector2 size)
            {
                _left = targetBounds.center.x - size.x / 2;
                _right = targetBounds.center.x + size.x / 2;
                _bottom = targetBounds.min.y;
                _top = targetBounds.min.y + size.y;

                Velocity = Vector2.zero;
                Centre = new Vector2((_left + _right) / 2, (_top + _bottom) / 2);
            }

            public void Update(Bounds targetBounds)
            {
                var shiftX = UpdateLeftAndRight(targetBounds);
                var shiftY = UpdateTopAndBottom(targetBounds);

                Centre = new Vector2((_left + _right) / 2, (_top + _bottom) / 2);
                Velocity = new Vector2(shiftX, shiftY);
            }

            private float UpdateTopAndBottom(Bounds targetBounds)
            {
                float shiftY = 0;

                if (targetBounds.min.y < _bottom)
                    shiftY = targetBounds.min.y - _bottom;
                else if (targetBounds.max.y > _top)
                    shiftY = targetBounds.max.y - _top;

                _top += shiftY;
                _bottom += shiftY;
                return shiftY;
            }

            private float UpdateLeftAndRight(Bounds targetBounds)
            {
                float shiftX = 0;

                if (targetBounds.min.x < _left)
                    shiftX = targetBounds.min.x - _left;
                else if (targetBounds.max.x > _right)
                    shiftX = targetBounds.max.x - _right;

                _left += shiftX;
                _right += shiftX;
                return shiftX;
            }
        }
        #endregion

        #region Cam Shake

        private void ChangeShake()
        {
            CamShake = !CamShake;
        }

        public void ShakeCam()
        {
            ChangeShake();
            Invoke("ChangeShake", _shakeDuration);
        }

        #endregion

    }
}