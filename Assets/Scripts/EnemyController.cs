using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent (typeof (WaypointController))]
    public class EnemyController : Controller2D
    {

        #region Variables
        [SerializeField]
        private Vector2 _deltaMovement;
        private WaypointController _waypoints;
        private bool _horDir;
        private bool _verDir;
        private float _hp;
        private Vector2 _oldPosition;
        public GameObject EnemyGraphics;
        public Animator GraphicsAnimator;
        public bool Alive = true ;

        #endregion

        #region Start
        public override void Start()
        {
            base.Start();
            SetDefaut();
            GraphicsAnimator = EnemyGraphics.GetComponent<Animator>();
        }

        private void SetDefaut()
        {
            _waypoints = GetComponent<WaypointController>();
            _hp = 1;
        }
        #endregion

        #region Update
        public new void Update()
        {
            _horDir = (_oldPosition.x < transform.position.x) ? true : false;
            _verDir = (_oldPosition.y < transform.position.y) ? true : false;
            _oldPosition = (Vector2)transform.position;
            _deltaMovement = _waypoints.CalculateWaypointMovement();
            EnemyMove(_deltaMovement);
            GraphicsAnimator.SetBool("horDir", _horDir);
            GraphicsAnimator.SetBool("verDir", _verDir);
            if (gameObject.tag == "DeadEnemy")
                Alive = false;
            GraphicsAnimator.SetBool("Alive", Alive);
        }
        #endregion
    }
}