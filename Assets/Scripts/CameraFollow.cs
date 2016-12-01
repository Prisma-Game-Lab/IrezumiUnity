using UnityEngine;
using System.Collections;

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

		private float _DefaultZoom; 
		[SerializeField]
		private float _TargetZoom;
		public float currentVelocityZoom;

        private bool DeadPlayer;
        Vector3 PlayerPosition;
        Vector3 NewPos;
        #endregion

        #region Start
        public void Start()
        {
            SetDefaut();
			_DefaultZoom = Camera.main.orthographicSize;
			_TargetZoom = _DefaultZoom;
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
		public void Update()
		{
			ZoomCam ();
		}


		#endregion

        #region LateUpdate
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
            if (DeadPlayer)
            {
                NewPos.x = Mathf.SmoothDamp(transform.position.x, PlayerPosition.x, ref _smoothLookVelocityX, 0.03f);
                NewPos.y = Mathf.SmoothDamp(transform.position.y, PlayerPosition.y, ref _smoothLookVelocityY, 0.03f);
                NewPos.z = -10;
                _TargetZoom = 4;
                transform.position = NewPos;
            }
            else
            {
               transform.position = (Vector3)focusPosition + Vector3.forward * -10;
            }
            
        }

        private void SetTargetLookAheadX()
        {
            if (_focusArea.Velocity.x != 0)
            {
                _lookAheadDirX = Mathf.Sign(_focusArea.Velocity.x);
                if (Mathf.Sign(Target.Input.x) == Mathf.Sign(_focusArea.Velocity.x) && Target.Input.x != 0)
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

		#region Cam Zoom

		private void ZoomCam()
		{
            float ZoomTime = 0.2f;
            if (DeadPlayer)
            {
                ZoomTime = 2;
            }
            Camera.main.orthographicSize = Mathf.SmoothDamp (Camera.main.orthographicSize, _TargetZoom, ref currentVelocityZoom, ZoomTime);
            
		}

		public void ChangeTargetZoom(float NewTargetZoom)
		{
			Debug.Log (NewTargetZoom);
			_TargetZoom = NewTargetZoom;
			Invoke ("DefaultTargetZoom",0.5F);
		}
		private void DefaultTargetZoom()
		{
			_TargetZoom = _DefaultZoom;
		}

        #endregion

        #region Dramatic Zoom
        public void DramaticZoom(Vector3 PPosition)
        {
            DeadPlayer = true;
            PlayerPosition = PPosition;
        }
        #endregion

    }
}