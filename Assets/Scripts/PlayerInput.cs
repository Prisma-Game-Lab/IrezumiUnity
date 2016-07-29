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

        private ParticleSystem ParticleSystem;
        private GameObject Trail;
        private TrailRenderer tr;
        private GameObject gameManager;
        private GameManager gm;

        [SerializeField]
        public bool TakingHit;
        #endregion

        #region Start
        public void Start()
        {
            Trail = GameObject.Find("FollowTrail");
            tr = Trail.GetComponent<TrailRenderer>();
            Player = GetComponent<Player>();
            gameManager = GameObject.Find("Game Manager");
            gm = gameManager.GetComponent<GameManager>();
            ParticleSystem = ParticleSpawn.GetComponent<ParticleSystem>();
            DeactivateParticle();
        }
        #endregion

        #region Update
        public void Update()
        {
            if (!gm.IsPaused())
            {
                SetDashCooldown();

                /*vector input stores players coordinates*/
                var directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                Player.CheckIfPlayerMoved(directionalInput);
                Player.SetDirectionalInput(directionalInput);

                if (!TakingHit)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                        Player.OnJumpInputDown();
                    if (Input.GetKeyUp(KeyCode.Space))
                        Player.OnJumpInputUp();
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (DashCooldown == 0)
                        {
                            DashCooldown = 1.5f;
                            Player.OnDashInput();
                            ActivateParticle();
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
            ParticleSystem.Play();
        }

        private void DeactivateParticle()
        {
            ParticleSystem.Stop();
        }
        #endregion
    }
}