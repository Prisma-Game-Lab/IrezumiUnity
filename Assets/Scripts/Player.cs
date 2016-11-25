using UnityEngine;

namespace Assets.Scripts
{

    [RequireComponent(typeof(FreezeFrame))]
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
		public float HpLostPerSecond;
		public float HpLostOnDash;
		public float HpRecoverOnKill;
        public Vector2 WallJumpClimb;
        public Vector2 WallJumpOff;
        public Vector2 WallJumpLeap;
        public Vector2 PushSpeed;
        public bool TakingHit;
        public bool IsInvulnerable;
        public bool IsDashing;
        public GameObject PlayerGraphics;
        public FreezeFrame FreezeFrame;
        [HideInInspector]
        public Vector2 Velocity;
        [HideInInspector]
        public float InkCollected;
        [HideInInspector]
        public bool FirstInput;

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
            // SetDefaut();
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
			FirstInput = false;

            MoveSpeed = 25;
            MaxjumpHeight = 9;
            MinjumpHeight = 1;
            TimeToJumpApex = .4f;
            WallSlideSpeed = 15;
            WallStickTime = .25f;
            DashSpeed = 45;
            DashTime = .36666f;
            Hp = 100;
			HpLostPerSecond = 5;
			HpLostOnDash = 10;
			HpRecoverOnKill = 20;
            InkCollected = 0;

            WallJumpClimb = new Vector2(48, 36); // 36, 28
            WallJumpOff = new Vector2(6, 5); // 36, 28
            WallJumpLeap = new Vector2(58, 48); // 45, 32
            PushSpeed = new Vector2(30, 30);

            TakingHit = false;
            IsInvulnerable = false;
            IsDashing = false;
            
            FreezeFrame = GetComponent<FreezeFrame>();
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
            if (!GameManager.Instance.IsPaused)
            {
                _wallSliding = false;
                _wallDirX = (_controller.Collisions.Left) ? -1 : 1;
                if (FirstInput)
                    Hp -= HpLostPerSecond*Time.deltaTime;

                if (Hp <= 0)
                {
                    Debug.Log("YOU ARE DEAD");
                    GameManager.Instance.EnablePause();
                }

                CalculateVelocity();
                HandleWallSliding();
                if (_grounded)
                    _graphicsAnimator.SetBool("LeavingWall", false);
                _grounded = false;
                _facingRight = _controller.Collisions.FaceDir == 1;

                SetGraphicsAnimatorConfigurations();

                _controller.Move(Velocity*Time.deltaTime, _directionalInput);

                ResetVerticalVelocity();

                _graphicsAnimator.SetBool("Grounded", _grounded);
            }
        }

        private void CalculateVelocity()
        {
            float targetVelocityX = 0;

			if (IsDashing)
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
			else//if (!IsDashing && !TakingHit) //afinal, se nao entrou nos outros 2 ifs este vale automaticamente
            {
                targetVelocityX = _directionalInput.x * MoveSpeed;
                Velocity.y += _gravity * Time.deltaTime;
                _firstTimeTakingHit = true;
            }

            if (Velocity.y < -70)
            {
                Velocity.y = -70;
            }

            Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.Collisions.Below) ? _accelerationTimeGrounded : _accelerationTimeAirborne);
            //_stopped = Velocity.x == 0; //Revisão Pietro 16/08
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
            //_graphicsAnimator.SetBool("Dashing", IsDashing);
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
		public void CheckIfPlayerMoved (Vector2 directionalInput){
			if (directionalInput != Vector2.zero && !FirstInput) 
				FirstInput = true;
		}

        public void SetDirectionalInput(Vector2 input)
        {
            _directionalInput = input;
			_stopped = input.x == 0; //Revisão Pietro 16/08
        }

        public void OnDashInput()
        {
			FirstInput = true;
            //IsDashing = true;
            _graphicsAnimator.SetBool("Dashing",true);
            //Invoke("ResetDashing", DashTime);
			Hp -= HpLostOnDash;
        }

        public void StartDash()
        {
            IsDashing = true;
        }

        public void ResetAllDashing()
        {
            IsDashing = false;
            _graphicsAnimator.SetBool("Dashing", false);
        }

        public void OnJumpInputDown()
		{
			FirstInput = true;
            if (_wallSliding)
            {
                _graphicsAnimator.SetBool("LeavingWall",true);
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
			FirstInput = true;
            if (Velocity.y > _minjumpVelocity)
                Velocity.y = _minjumpVelocity;
        }
         #endregion

		#region Others
		/// <summary>
		/// Recovers the hp.
		/// </summary>
		public void RecoverHp()
		{
		    InkCollected += HpRecoverOnKill;
			Hp += HpRecoverOnKill;
			if (Hp > 100) Hp = 100;
		}

        public void Damage(float dmg)
        {
            Hp -= dmg;
            if (Hp < 0)
                Hp = 0;
        }
		#endregion

        #region Not used
      

        private void CalculatePushSpeed()
        {
            PushSpeed.y = -_gravity * _controller.TimeToRecover;
            PushSpeed.x = 5;
        }
        #endregion

		#region Used by other classes
		/// <summary>
		/// Forces the push.
		/// </summary>
		/// <param name="velocity">Velocity.</param>
		public void ForcePush(Vector2 velocity, bool standingOnPlatform = false){
            if (_wallSliding)
            {
                Velocity.x = -_wallDirX * 1.75f * WallJumpLeap.x;
                Velocity.y = WallJumpLeap.y;
            }
            else
            {
                _controller.Move(velocity * Time.deltaTime, standingOnPlatform);
            }
		}
		#endregion
    }
}


