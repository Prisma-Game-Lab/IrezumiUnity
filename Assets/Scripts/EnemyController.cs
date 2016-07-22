using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent (typeof (WaypointController))]
    public class EnemyController : Controller2D
    {

        #region Variables
        private Vector2 _velocity;
        private WaypointController _waypoints;
        private int _faceDir;
        private int _verDir;
        private bool _change;
        private float _hp;
        private float _velocityXSmoothing;
        private float _velocityYSmoothing;
        public float MoveSpeedX;
        public float MoveSpeedY;
        public float AccelerationTimeX;
        public float AccelerationTimeY;
        #endregion

        #region Start
        public override void Start()
        {
            base.Start();
            SetDefaut();
        }

        private void SetDefaut()
        {
            _waypoints = GetComponent<WaypointController>();
            _hp = 1;
            AccelerationTimeX = .04f;
            AccelerationTimeY = .04f;
            _faceDir = 1;
            _verDir = 0;
        }
        #endregion

        #region Update
        public new void Update()
        {
            Vector2 velocity = _waypoints.CalculateWaypointMovement();
            EnemyMove(velocity);
            if (Collisions.Left || Collisions.Right)
                ChangeDirection();
        }

        private void ChangeDirection()
        {
            _change = false;
            _faceDir = _faceDir == 1 ? -1 : 1;
        }
        #endregion
    }
}