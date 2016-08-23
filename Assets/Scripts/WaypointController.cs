using UnityEngine;

namespace Assets.Scripts
{
    public class WaypointController : RaycastController
    {

        #region Variables
        public Vector3[] LocalWaypoints;
        public bool Cyclic;
        public float Speed;
        public float WaitTime;
        [Range(0, 2)]
        public float EaseAmount;

        protected int FromWaypointIndex;
        protected int ToWaypointIndex;
        protected float PercentBetweenWaypoints;
        protected float NextMoveTime;
        protected Vector3[] GlobalWaypoints;
        #endregion

        #region Start
        public override void Start()
        {
            base.Start();
            GlobalWaypoints = new Vector3[LocalWaypoints.Length];
            for (var i = 0; i < LocalWaypoints.Length; i++)
                GlobalWaypoints[i] = LocalWaypoints[i] + transform.position;
        }
        #endregion

        #region Update
        public void Update () {
            UpdateRaycastOrigins();
        }
        
        #endregion

        #region Draw
        public void OnDrawGizmos()
        {
            if (LocalWaypoints != null)
            {
                Gizmos.color = Color.red;
                const float size = .3f;

                for (var i = 0; i < LocalWaypoints.Length; i++)
				{
                    Vector3 globalWaypointPos = (Application.isPlaying) ? GlobalWaypoints[i] : LocalWaypoints[i] + transform.position;
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }
            }
        }
        #endregion

        #region WayPoint
        public Vector3 CalculateWaypointMovement()
        {
            if (Time.time < NextMoveTime)
                return Vector3.zero;

            var easedPercentBetweenWaypoints = GetEasedPercentBetweenWaypoints();
            Vector3 newPos = Vector3.Lerp(GlobalWaypoints[FromWaypointIndex], GlobalWaypoints[ToWaypointIndex], easedPercentBetweenWaypoints);

            if (PercentBetweenWaypoints >= 1)
                ReachWaypoint();

            return newPos - transform.position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private float GetEasedPercentBetweenWaypoints()
        {
            FromWaypointIndex %= GlobalWaypoints.Length;
            ToWaypointIndex = (FromWaypointIndex + 1) % GlobalWaypoints.Length;
            var distanceBetweenWaypoints = Vector3.Distance(GlobalWaypoints[FromWaypointIndex], GlobalWaypoints[ToWaypointIndex]);

            PercentBetweenWaypoints += Time.deltaTime * Speed / distanceBetweenWaypoints;
            PercentBetweenWaypoints = Mathf.Clamp01(PercentBetweenWaypoints);
            var easedPercentBetweenWaypoints = Ease(PercentBetweenWaypoints);
            return easedPercentBetweenWaypoints;
        }

        /// <summary>
        /// Platforms reached waypoint
        /// </summary>
        private void ReachWaypoint()
        {
            PercentBetweenWaypoints = 0;
            FromWaypointIndex++;

            if (!Cyclic)
                MakeNonCyclicMovement();

            NextMoveTime = Time.time + WaitTime;
        }

        /// <summary>
        /// Resets FromWaypointIndex to zero and reverse array of globalWaypoints if the index is out of range 
        /// </summary>
        private void MakeNonCyclicMovement()
        {
            if (FromWaypointIndex >= GlobalWaypoints.Length - 1)
            {
                FromWaypointIndex = 0;
                System.Array.Reverse(GlobalWaypoints);
            }
        }

        /// <summary>
        /// Eases the platform movement
        /// </summary>
        /// <param name="x"></param>
        /// <returns>eased movement</returns>
        private float Ease(float x)
        {
            float a = EaseAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }
        #endregion

    }
}
