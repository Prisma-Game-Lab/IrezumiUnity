using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Player))]
    public class PlayerInput : MonoBehaviour
    {
        #region Variables
        public Player Player;
        public float DashCooldown;

        GameObject gameManager;
        GameManager gm;

        [SerializeField]
        public bool TakingHit;
        #endregion

        #region Start
        public void Start()
        {
            Player = GetComponent<Player>();
            gameManager = GameObject.Find("Game Manager");
            gm = gameManager.GetComponent<GameManager>();
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
    }
}