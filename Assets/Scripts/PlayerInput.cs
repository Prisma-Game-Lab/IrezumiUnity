using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Player))]
    public class PlayerInput : MonoBehaviour
    {
        #region Variables
        public Player Player;
        public float DashCooldown;
        public GameObject ParticleSpawn;

        private ParticleSystem _particleSystem;
        private GameObject _trail;
        private TrailRenderer _tr;
        private GameManager _gameManager;

        [SerializeField]
        public bool TakingHit;
        #endregion

        #region Start
        public void Start()
        {
            _trail = GameObject.Find("FollowTrail");
            _tr = _trail.GetComponent<TrailRenderer>();
            Player = GetComponent<Player>();
            _gameManager = GameManager.Instance;
            _particleSystem = ParticleSpawn.GetComponent<ParticleSystem>();
            DeactivateParticle();
        }
        #endregion

        #region Update
        public void Update()
        {
            if (!_gameManager.IsPaused)
            {
                SetDashCooldown();

                /*vector input stores players coordinates*/
                
                var directionalInput = InputManager.Directional_Input();
                //Debug.Log(directionalInput);
                if (!Player.ALIVE)
                {
                    directionalInput = Vector2.zero;
                }

                Player.CheckIfPlayerMoved(directionalInput);                
                
                if(Player.WallJumping)
                {
                   Player.SetDirectionalInput(-directionalInput);
                }
                else
                {
                    Player.SetDirectionalInput(directionalInput);
                }           
                

                if (!TakingHit && Player.ALIVE)
                {
                    if (InputManager.Jump_InputDown())
                        Player.OnJumpInputDown();
                    if (InputManager.Jump_InputUp())
                        Player.OnJumpInputUp();
                    if (InputManager.Dash_Input())
                    {
                        if (DashCooldown == 0)
                        {
                            DashCooldown = 1.5f;
                            Player.OnDashInput();
                            // ActivateParticle();
                            Invoke("DeactivateParticle", 0.32f);
                        }
                    }
                }
                Player.TakingHit = TakingHit;
            }
        }

        private void SetDashCooldown()
        {
            DashCooldown -= Time.deltaTime;
            if (DashCooldown < 0)
                DashCooldown = 0;
        }
        #endregion

        #region Hit
        public bool GetHit()
        {
            return TakingHit;
        }
        #endregion

        #region Paticles

        private void ActivateParticle()
        {
            _particleSystem.Play();
        }

        private void DeactivateParticle()
        {
            _particleSystem.Stop();
        }
        #endregion
    }
}