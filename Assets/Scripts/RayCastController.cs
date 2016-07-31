using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastController : MonoBehaviour
    {
        #region Variables
        private float _dstBetweeenRays;

        public LayerMask CollisionMask;
        public float SkinWidth;
        [HideInInspector]
        public int HorizontalRayCount;
        [HideInInspector]
        public int VerticalRayCount;

        [HideInInspector]
        public float HorizontalRaySpacing;
        [HideInInspector]
        public float VerticalRaySpacing;

        [HideInInspector]
        public BoxCollider2D Collider;
        [HideInInspector]
        public RaycastOriginsStruct RaycastOrigins;
        #endregion

        #region Start
        public virtual void Awake()
        {
            SetDefaut();
            Collider = GetComponent<BoxCollider2D>();
            CalculateRaySpacing();
        }

        private void SetDefaut()
        {
            SkinWidth = .015f;
            _dstBetweeenRays = .25f;
        }

        public virtual void Start()
        {
            CalculateRaySpacing();
        }
        #endregion

        #region RaySpacing
        public void CalculateRaySpacing()
        {
            var bounds = Collider.bounds;
            bounds.Expand(SkinWidth * -2);

            var boundsWidth = bounds.size.x;
            var boundsHieght = bounds.size.y;

            HorizontalRayCount = Mathf.RoundToInt(boundsHieght / _dstBetweeenRays);
            VerticalRayCount = Mathf.RoundToInt(boundsWidth / _dstBetweeenRays);

            HorizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
            VerticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
        }
        #endregion

        #region RayCastOrigins
        public void UpdateRaycastOrigins()
        {
            Bounds bounds = Collider.bounds;
            bounds.Expand(SkinWidth * -2);

            RaycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            RaycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            RaycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            RaycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        
        }
        #endregion

        #region Structs
        public struct RaycastOriginsStruct
        {
            public Vector2 TopLeft, TopRight;
            public Vector2 BottomLeft, BottomRight;
        }
        #endregion
    }
}