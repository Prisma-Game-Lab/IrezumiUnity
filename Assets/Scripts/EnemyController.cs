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
        private Animator _graphicsAnimator;


        #endregion

        #region Start
        public override void Start()
        {
            base.Start();
            SetDefaut();
            _graphicsAnimator = EnemyGraphics.GetComponent<Animator>();
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
            _graphicsAnimator.SetBool("horDir", _horDir);
            _graphicsAnimator.SetBool("verDir", _verDir);

        }
        #endregion
    }
}