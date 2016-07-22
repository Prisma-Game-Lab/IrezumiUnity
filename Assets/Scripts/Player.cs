﻿using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent (typeof (Controller2D))]
    public class Player : MonoBehaviour
    {

        #region Variables
        public float MoveSpeed;
        public float MaxjumpHeight;
        public float MinjumpHeight;
        public float TimeToJumpApex;
        public float WallSlideSpeed;
        public float WallStickTime;
        public float DashSpeed;
        public float DashTime;
        public float Hp;
        public Vector2 WallJumpClimb;
        public Vector2 WallJumpOff;
        public Vector2 WallJumpLeap;
        public Vector2 PushSpeed;
        public bool TakingHit;
        public bool IsInvulnerable;
        public bool IsDashing;
        public GameObject PlayerGraphics;
        [HideInInspector]
        public Vector2 Velocity;

        private float _timeToWallUnstick;
        private float _accelerationTimeAirborne;
        private float _accelerationTimeGrounded;
        private float _maxjumpVelocity;
        private float _minjumpVelocity;
        private float _velocityXSmoothing;
        private bool _wallSliding;
        private bool _firstTimeTakingHit;
        private bool _facingRight;
        private bool _stopped;
        private bool _grounded;
        private int _wallDirX;
        private Controller2D _controller;
        private Vector2 _directionalInput;
        private Animator _graphicsAnimator;
        [SerializeField]
        private float _gravity;
        #endregion

        #region Start
        public void Start()
        {
            _controller = GetComponent<Controller2D>();
            _graphicsAnimator = PlayerGraphics.GetComponent<Animator>();
            SetDefaut();
            SetGravityAndVelocityEquations();
        }

        /// <summary>
        /// Sets defaut values
        /// </summary>
        private void SetDefaut()
        {
            _accelerationTimeAirborne = .04f;
            _accelerationTimeGrounded = .01f;
            _firstTimeTakingHit = true;

            MoveSpeed = 16;
            MaxjumpHeight = 9;
            MinjumpHeight = 1;
            TimeToJumpApex = .4f;
            WallSlideSpeed = 3;
            WallStickTime = .25f;
            DashSpeed = 30;
            DashTime = .3f;
            Hp = 100;

            WallJumpClimb = new Vector2(36, 28);
            WallJumpOff = new Vector2(2, 3);
            WallJumpLeap = new Vector2(45, 32);
            PushSpeed = new Vector2(20, 20);

            TakingHit = false;
            IsInvulnerable = false;
            IsDashing = false;
        }

        private void SetGravityAndVelocityEquations()
        {
            _gravity = -(2 * MaxjumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
            _maxjumpVelocity = Mathf.Abs(_gravity) * TimeToJumpApex;
            _minjumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * MinjumpHeight);
        }
        #endregion

        #region Update
        public void Update()
        {
            _wallSliding = false;
            _wallDirX = (_controller.Collisions.Left) ? -1 : 1;

            CalculateVelocity();
            HandleWallSliding();

            _grounded = false;
            _facingRight = _controller.Collisions.FaceDir == 1;

            SetGraphicsAnimatorConfigurations();

            _controller.Move(Velocity * Time.deltaTime, _directionalInput);

            ResetVerticalVelocity();

            _graphicsAnimator.SetBool("Grounded", _grounded);
        }

        private void CalculateVelocity()
        {
            float targetVelocityX = 0;

            if (!IsDashing && !TakingHit)
            {
                targetVelocityX = _directionalInput.x * MoveSpeed;
                Velocity.y += _gravity * Time.deltaTime;
                _firstTimeTakingHit = true;
            }
            else if (IsDashing)
            {
                Velocity.y = 0;
                targetVelocityX = _controller.Collisions.FaceDir * DashSpeed;
                _firstTimeTakingHit = true;
            }
            else if (TakingHit)
            {
                var pushDir = _controller.Collisions.FaceDir;
                if (_firstTimeTakingHit)
                {
                    pushDir = -pushDir;
                    _firstTimeTakingHit = false;
                    Velocity.y = PushSpeed.y;
                }
                targetVelocityX = pushDir * PushSpeed.x;
                Velocity.y += _gravity * Time.deltaTime;
            }

            Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.Collisions.Below) ? _accelerationTimeGrounded : _accelerationTimeAirborne);
            _stopped = Velocity.x == 0;
        }

        private void HandleWallSliding()
        {
            _wallDirX = (_controller.Collisions.Left) ? -1 : 1;
            _wallSliding = false;

            if ((_controller.Collisions.Left || _controller.Collisions.Right) && !_controller.Collisions.Below && Velocity.y < 0)
            {
                _wallSliding = true;

                if (Velocity.y < -WallSlideSpeed)
                    Velocity.y = -WallSlideSpeed;

                if (_timeToWallUnstick > 0)
                {
                    _velocityXSmoothing = 0;
                    Velocity.x = 0;

                    if (_directionalInput.x != _wallDirX && _directionalInput.x != 0)
                        _timeToWallUnstick -= Time.deltaTime;
                    else
                        _timeToWallUnstick = WallStickTime;
                }
                else
                    _timeToWallUnstick = WallStickTime;
            }

        }

        private void SetGraphicsAnimatorConfigurations()
        {
            _graphicsAnimator.SetFloat("Velocity.x", Velocity.x);
            _graphicsAnimator.SetFloat("Velocity.y", Velocity.y + 1.9f);
            _graphicsAnimator.SetBool("FacingRight", _facingRight);
            _graphicsAnimator.SetBool("Stopped", _stopped);
            _graphicsAnimator.SetBool("Dashing", IsDashing);
            _graphicsAnimator.SetBool("WallSliding", _wallSliding);
        }

        /// <summary>
        /// Resets vertical velocity in case player is colliding with ground or roof
        /// </summary>
        private void ResetVerticalVelocity()
        {
            if (_controller.Collisions.Above || _controller.Collisions.Below)
            {
                Velocity.y = 0;
                if (_controller.Collisions.Below)
                    _grounded = true;
            }
        }
        #endregion

        #region Player Input
        public void SetDirectionalInput(Vector2 input)
        {
            _directionalInput = input;
        }

        public void OnDashInput()
        {
            IsDashing = true;
            Invoke("ResetDashing", DashTime);
        }
       
        public void OnJumpInputDown()
        {
            if (_wallSliding)
            {
                /*player trying to move the same direction as the wall its facing and move away from the wall*/
                if (_wallDirX == _directionalInput.x)
                {
                    Velocity.x = -_wallDirX * WallJumpClimb.x;
                    Velocity.y = WallJumpClimb.y;
                }
                    /*player doesnt press any key and fall off the wall*/
                else if (_directionalInput.x == 0)
                {
                    Velocity.x = -_wallDirX * WallJumpOff.y;
                    Velocity.y = WallJumpOff.y;
                }
                    /*player trying to move opposite direction */
                else
                {
                    Velocity.x = -_wallDirX * WallJumpLeap.x;
                    Velocity.y = WallJumpLeap.y;
                }
            }
            if (_controller.Collisions.Below)
                Velocity.y = _maxjumpVelocity;
        }

        public void OnJumpInputUp()
        {
            if (Velocity.y > _minjumpVelocity)
                Velocity.y = _minjumpVelocity;
        }
         #endregion

        #region Not used
        public void ResetDashing()
        {
            IsDashing = false;
        }

        private void CalculatePushSpeed()
        {
            PushSpeed.y = -_gravity * _controller.TimeToRecover;
            PushSpeed.x = 5;
        }
        #endregion
    }
}
