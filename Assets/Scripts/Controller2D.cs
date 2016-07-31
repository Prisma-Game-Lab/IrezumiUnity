using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent (typeof (BoxCollider2D))]
    [RequireComponent(typeof(FreezeFrame))]
    public class Controller2D : RaycastController
    {

        #region Variables
        private float _maxClimbAngle;
        private float _maxDescendAngle;
        private GameObject MainCamera;
        private CameraFollow CamScript;
                       
        public LayerMask InteractiveMask;
        public CollisionInfo Collisions;
        public PlayerInput PInput;
        public FreezeFrame FreezeFrame;
        public float TimeToRecover;
        public float InvulnerabilityTime;
        public float PauseDuration;
        [HideInInspector]
        public Vector2 PlayerInput;
        protected bool _isEnemyDead;
        #endregion     
                       
        #region Start
        public override void Start()
        {
            SetDefaut();
            base.Start();
            Collisions.FaceDir = 1;

            Collider = GetComponent<BoxCollider2D> ();
            FreezeFrame = GetComponent<FreezeFrame>();
            CalculateRaySpacing ();

            MainCamera = GameObject.Find("Main Camera");
            CamScript = MainCamera.GetComponent<CameraFollow>();

        }

        private void SetDefaut()
        {
            _maxClimbAngle = 80;
            _maxDescendAngle = 80;
            TimeToRecover = .18f;
            InvulnerabilityTime = 2;
            InteractiveMask = LayerMask.GetMask("Enemy");
        }
        #endregion          
                       
        #region Update
        public void Update()
        {
            EnemyCollisions();
        }
        #endregion

        #region EnemyCollisions
        public void EnemyCollisions()
        {
            var rayLength = 2 * SkinWidth;
            HorizontalEnemyCollision(rayLength);
            VerticalEnemyCollision(rayLength);
        }

        private void HorizontalEnemyCollision(float rayLength)
        {
            for (var i = 0; i < HorizontalRayCount; i++)
            {
                if (LeftEnemyCollision(rayLength, i)) 
                    continue;
                RightEnemyCollision(rayLength, i);
            }
        }

        private bool LeftEnemyCollision(float rayLength, int i)
        {
            Vector2 rayOrigin1 = RaycastOrigins.BottomLeft;
            rayOrigin1 += Vector2.up*(HorizontalRaySpacing*i);
            Debug.DrawRay(rayOrigin1, Vector2.left*rayLength, Color.green);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin1, Vector2.left, rayLength, InteractiveMask);
            if (hit)
            {
                if (hit.collider.tag == "Enemy" && hit.distance == 0 && !PInput.GetHit() && !PInput.Player.IsInvulnerable)
                {
					if (PInput.Player.IsDashing)
					{
                        DestroyEnemy(hit);
                    }
                    else
                    {
                        SetPlayerWasHitAndIsInvulnerable();
                    }
                    return true;
                }
            }
            return false;
        }

        private void RightEnemyCollision(float rayLength, int i)
        {
            Vector2 rayOrigin2 = RaycastOrigins.BottomRight;
            rayOrigin2 += Vector2.up*(HorizontalRaySpacing*i);
            Debug.DrawRay(rayOrigin2, Vector2.right*rayLength, Color.cyan);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin2, Vector2.right, rayLength, InteractiveMask);
            if (hit)
            {
                if (hit.collider.tag == "Enemy" && hit.distance == 0 && !PInput.GetHit() && !PInput.Player.IsInvulnerable)
                {
					if (PInput.Player.IsDashing) 
					{
                        DestroyEnemy(hit);
                    }
                    else
                        SetPlayerWasHitAndIsInvulnerable();
                }
            }
        }

        private void DestroyEnemy(RaycastHit2D hit)
        {
            _isEnemyDead = true;
            CamScript.ShakeCam();
            var enemyHit = hit.collider.gameObject;
            enemyHit.layer = 10;
            enemyHit.tag = "DeadEnemy";
            PInput.Player.RecoverHp();
        }

        private void VerticalEnemyCollision(float rayLength)
        {
            for (var i = 0; i < VerticalRayCount; i++)
            {
                if (DownEnemyCollision(rayLength, i)) 
                    continue;

                UpEnemyCollision(rayLength, i);
            }
        }

        private bool DownEnemyCollision(float rayLength, int i)
        {
            Vector2 rayOrigin1 = RaycastOrigins.BottomLeft;
            rayOrigin1 += Vector2.right*(VerticalRaySpacing*i);
            Debug.DrawRay(rayOrigin1, Vector2.down*rayLength, Color.gray);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin1, Vector2.down, rayLength, InteractiveMask);
            if (hit)
            {
                if (hit.collider.tag == "Enemy" && hit.distance == 0)
                {
                    if (PInput.Player.IsDashing)
                        DestroyEnemy(hit);
                    return true;
                }
            }
            return false;
        }

        private void UpEnemyCollision(float rayLength, int i)
        {
            Vector2 rayOrigin2 = RaycastOrigins.TopLeft;
            rayOrigin2 += Vector2.right*(VerticalRaySpacing*i);
            Debug.DrawRay(rayOrigin2, Vector2.up*rayLength, Color.blue);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin2, Vector2.up, rayLength, InteractiveMask);

            if (hit)
            {
                if (hit.collider.tag == "Enemy" && hit.distance == 0)
                {
                    if (PInput.Player.IsDashing)
                        DestroyEnemy(hit);
                }
            }
        }

        private void SetPlayerWasHitAndIsInvulnerable()
        {
            ChangeHit();
            Invoke("ChangeHit", TimeToRecover);
            SetInvulnerability();
            Invoke("SetInvulnerability", InvulnerabilityTime);
        }
        public void ChangeHit()
        {
            PInput.TakingHit = !PInput.TakingHit;
        }

        private void SetInvulnerability()
        {
            PInput.Player.IsInvulnerable = !PInput.Player.IsInvulnerable;
        }
        #endregion

        #region Move
        public void EnemyMove(Vector2 deltaMoveE)
        {
            Move(deltaMoveE, Vector2.zero, false);
        }

        public void Move(Vector2 deltaMove, bool standingOnPlatform)
        {
            Move(deltaMove, Vector2.zero, standingOnPlatform);
        }

        public void Move(Vector2 deltaMove, Vector2 input, bool standingOnPlatform = false)
        {
            UpdateConfigurations(deltaMove, input);

            if (deltaMove.x != 0)
                Collisions.FaceDir = (int)Mathf.Sign(deltaMove.x);

            if (deltaMove.y < 0)
                DescendSlope(ref deltaMove);

            CheckForCollisions(ref deltaMove);

            transform.Translate(deltaMove);

            if (standingOnPlatform)
                Collisions.Below = true;
        }

        private void UpdateConfigurations(Vector2 deltaMove, Vector2 input)
        {
            UpdateRaycastOrigins();
            Collisions.Reset();
            Collisions.DeltaMoveOld = deltaMove;
            PlayerInput = input;
        }
        #endregion

        #region Collisions
        /// <summary>
        /// Checks for collisions and update deltaMove
        /// </summary>
        /// <param name="deltaMove"></param>
        private void CheckForCollisions(ref Vector2 deltaMove)
        {
            HorizontalCollisions(ref deltaMove);

            if (deltaMove.y != 0)
                VerticalCollisions(ref deltaMove);
        }

        #region Horizontal Collision
        /// <summary>
        /// Horizontal Collisions
        /// </summary>
        /// <param name="deltaMove"></param>
        private void HorizontalCollisions(ref Vector2 deltaMove) {
            var directionX = Collisions.FaceDir;
            var rayLength = Mathf.Abs (deltaMove.x) + SkinWidth;
		
            if (Mathf.Abs(deltaMove.x) < SkinWidth)
                rayLength = 2 * SkinWidth;

            for (var i = 0; i < HorizontalRayCount; i ++) {
                var rayOrigin = GetRayOriginForHorizontalCollisions(directionX, i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);
                if (hit) 
                    rayLength = HitInHorizontalCollision(ref deltaMove, hit, i, directionX, rayLength);
            }
        }

        private Vector2 GetRayOriginForHorizontalCollisions(int directionX, int i)
        {
            Vector2 rayOrigin = (directionX == -1) ? RaycastOrigins.BottomLeft : RaycastOrigins.BottomRight;
            rayOrigin += Vector2.up*(HorizontalRaySpacing*i);
            Debug.DrawRay(rayOrigin, Vector2.right*directionX, Color.red);
            return rayOrigin;
        }

        private float HitInHorizontalCollision(ref Vector2 deltaMove, RaycastHit2D hit, int i, int directionX, float rayLength)
        {
            if (hit.distance == 0)
                return rayLength;

            var slopeAngle = CheckIfClimbingSlopeForHorizontalCollision(ref deltaMove, hit, i, directionX);

            if (!Collisions.ClimbingSlope || slopeAngle > _maxClimbAngle)
            {
                deltaMove.x = (hit.distance - SkinWidth)*directionX;
                rayLength = hit.distance;

                if (Collisions.ClimbingSlope)
                    deltaMove.y = Mathf.Tan(Collisions.SlopeAngle*Mathf.Deg2Rad)*Mathf.Abs(deltaMove.x);

                Collisions.Left = directionX == -1;
                Collisions.Right = directionX == 1;
            }
            return rayLength;
        }

        private float CheckIfClimbingSlopeForHorizontalCollision(ref Vector2 deltaMove, RaycastHit2D hit, int i, int directionX)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (i == 0 && slopeAngle <= _maxClimbAngle)
            {
                if (Collisions.DescendingSlope)
                {
                    Collisions.DescendingSlope = false;
                    deltaMove = Collisions.DeltaMoveOld;
                }

                float distanceToSlopeStart = 0;
                if (slopeAngle != Collisions.SlopeAngleOld)
                {
                    distanceToSlopeStart = hit.distance - SkinWidth;
                    deltaMove.x -= distanceToSlopeStart*directionX;
                }
                ClimbSlope(ref deltaMove, slopeAngle);
                deltaMove.x += distanceToSlopeStart*directionX;
            }
            return slopeAngle;
        }
        #endregion

        #region Vertical Collisions
        /// <summary>
        /// Vertical Collisions
        /// </summary>
        /// <param name="deltaMove"></param>
        private void VerticalCollisions(ref Vector2 deltaMove) {
            var directionY = Mathf.Sign (deltaMove.y);
            var rayLength = Mathf.Abs (deltaMove.y) + SkinWidth;

            for (var i = 0; i < VerticalRayCount; i ++) {
                var rayOrigin = GetRayOriginForVerticalCollisions(deltaMove, directionY, i, rayLength);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, CollisionMask);

                if (hit) 
                    rayLength = HitInVerticalCollision(ref deltaMove, hit, directionY, rayLength);
            }
            CheckIfClimbingSlopeForVerticalCollision(ref deltaMove);
        }

        private Vector2 GetRayOriginForVerticalCollisions(Vector2 deltaMove, float directionY, int i, float rayLength)
        {
            Vector2 rayOrigin = (directionY == -1) ? RaycastOrigins.BottomLeft : RaycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (VerticalRaySpacing * i + deltaMove.x);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
            return rayOrigin;
        }

        private float HitInVerticalCollision(ref Vector2 deltaMove, RaycastHit2D hit, float directionY, float rayLength)
        {
            if (hit.collider.tag == "CanPass")
            {
                if (directionY == 1 || hit.distance == 0)
                    return rayLength;
                if (Collisions.PassingPlatform)
                    return rayLength;
                if (PlayerInput.y == -1)
                {
                    Collisions.PassingPlatform = true;
                    Invoke("ResetPassingPlatform", .4f);
                    return rayLength;
                }
            }

            deltaMove.y = (hit.distance - SkinWidth)*directionY;
            rayLength = hit.distance;

            if (Collisions.ClimbingSlope)
                deltaMove.x = deltaMove.y/Mathf.Tan(Collisions.SlopeAngle*Mathf.Deg2Rad)*Mathf.Sign(deltaMove.x);

            Collisions.Below = directionY == -1;
            Collisions.Above = directionY == 1;
            return rayLength;
        }

        private void CheckIfClimbingSlopeForVerticalCollision(ref Vector2 deltaMove)
        {
            if (Collisions.ClimbingSlope)
            {
                var directionX = Mathf.Sign(deltaMove.x);
                var rayLength = Mathf.Abs(deltaMove.x) + SkinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? RaycastOrigins.BottomLeft : RaycastOrigins.BottomRight) +
                                    Vector2.up * deltaMove.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != Collisions.SlopeAngle)
                    {
                        deltaMove.x = (hit.distance - SkinWidth) * directionX;
                        Collisions.SlopeAngle = slopeAngle;
                    }
                }
            }
        }
        
        #endregion

        #region Slope
        void ClimbSlope(ref Vector2 deltaMove, float slopeAngle) {
            float moveDistance = Mathf.Abs (deltaMove.x);
            float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (deltaMove.y <= climbVelocityY) {
                deltaMove.y = climbVelocityY;
                deltaMove.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (deltaMove.x);
                Collisions.Below = true;
                Collisions.ClimbingSlope = true;
                Collisions.SlopeAngle = slopeAngle;
            }
        }

        void DescendSlope(ref Vector2 deltaMove) {
            float directionX = Mathf.Sign (deltaMove.x);
            Vector2 rayOrigin = (directionX == -1) ? RaycastOrigins.BottomRight : RaycastOrigins.BottomLeft;
            RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, CollisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= _maxDescendAngle) {
                    if (Mathf.Sign(hit.normal.x) == directionX) {
                        if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaMove.x)) {
                            float moveDistance = Mathf.Abs(deltaMove.x);
                            float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            deltaMove.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (deltaMove.x);
                            deltaMove.y -= descendVelocityY;

                            Collisions.SlopeAngle = slopeAngle;
                            Collisions.DescendingSlope = true;
                            Collisions.Below = true;
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Not used
        private void ResetPassingPlatform()
        {
            Collisions.PassingPlatform = false;
        }
        #endregion

        #region Structs
        public struct CollisionInfo {
            public bool Above, Below;
            public bool Left, Right;

            public bool ClimbingSlope;
            public bool DescendingSlope;
            public float SlopeAngle, SlopeAngleOld;
            public Vector2 DeltaMoveOld;
            public int FaceDir;
            public bool PassingPlatform;

            public void Reset() {
                Above = Below = false;
                Left = Right = false;
                ClimbingSlope = false;
                DescendingSlope = false;

                SlopeAngleOld = SlopeAngle;
                SlopeAngle = 0;
            }
        }
        #endregion 

    }
}
