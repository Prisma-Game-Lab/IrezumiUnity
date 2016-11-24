using UnityEngine;

namespace Assets.Scripts
{
    public class JumpingEnemyController : RaycastController {

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
            UpdateRaycastOrigins();
            if (this.tag == "DeadEnemy")
            {
                Destroy(this.gameObject);
            }
        }

        public bool HitVertical(int directionY)
        {
            for (var i = 0; i < VerticalRayCount; i++)
            {
                var rayLength = SkinWidth;

                Vector2 rayOrigin = (directionY == -1) ? RaycastOrigins.BottomLeft : RaycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (VerticalRaySpacing * i);
                Debug.DrawRay(rayOrigin, Vector2.up * directionY , Color.green);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, CollisionMask);
                if (hit)
                    return true;
            }
            return false;
        }

        public bool HitHorizontal(int directionX)
        {
            for (var i = 0; i < HorizontalRayCount; i++)
            {
                var rayLength = SkinWidth;

                Vector2 rayOrigin = (directionX == -1) ? RaycastOrigins.BottomLeft : RaycastOrigins.BottomRight;
                rayOrigin += Vector2.up * (HorizontalRaySpacing * i);
                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);
                if (hit)
                    return true;
            }
            return false;
        }
    }
}
