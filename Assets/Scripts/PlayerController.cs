using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : Controller2D {
        
        private CameraFollow _camScript;

        public PlayerInput PInput;

        public override void Start()
        {
            base.Start();
            _camScript = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        }

        #region Player InteractiveCollisions
        public override bool LeftInteractiveCollision(float rayLength, int i)
        {
            Vector2 rayOrigin1 = RaycastOrigins.BottomLeft;
            rayOrigin1 += Vector2.up * (HorizontalRaySpacing * i);
            Debug.DrawRay(rayOrigin1, Vector2.left * rayLength, Color.green);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin1, Vector2.left, rayLength, InteractiveMask);
            if (hit)
            {
                if ((hit.collider.tag == "Enemy"|| hit.collider.tag == "FlyingEnemy") && hit.distance == 0 && !PInput.GetHit() && !PInput.Player.IsInvulnerable)
                {
                    if (PInput.Player.IsDashing)
                    {
                        HitEnemy(hit);		
                    }
                    else
                    {
                        PInput.Player.Damage(20);
                        SetPlayerWasHitAndIsInvulnerable();
                    }
                    return true;
                }
                else if (hit.collider.tag == "Trap" && hit.distance == 0 && !PInput.GetHit() && !PInput.Player.IsInvulnerable)
                {
                    PInput.Player.Damage(20);
                    SetPlayerWasHitAndIsInvulnerable();
                }
            }
            return false;
        }

        public override void RightInteractiveCollision(float rayLength, int i)
        {
            Vector2 rayOrigin2 = RaycastOrigins.BottomRight;
            rayOrigin2 += Vector2.up * (HorizontalRaySpacing * i);
            Debug.DrawRay(rayOrigin2, Vector2.right * rayLength, Color.cyan);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin2, Vector2.right, rayLength, InteractiveMask);
            if (hit)
            {
                if ((hit.collider.tag == "Enemy" || hit.collider.tag == "FlyingEnemy") && hit.distance == 0 && !PInput.GetHit() && !PInput.Player.IsInvulnerable)
                {
                    if (PInput.Player.IsDashing)
                    {
                        HitEnemy(hit);
                    }
                    else
                    {
                        PInput.Player.Damage(20);
                        SetPlayerWasHitAndIsInvulnerable();
                    }
                }
                else if (hit.collider.tag == "Trap" && hit.distance == 0)
                {
                    PInput.Player.Damage(20);
                    SetPlayerWasHitAndIsInvulnerable();
                }
            }
        }

        public override bool DownInteractiveCollision(float rayLength, int i)
        {
            Vector2 rayOrigin1 = RaycastOrigins.BottomLeft;
            rayOrigin1 += Vector2.right * (VerticalRaySpacing * i);
            Debug.DrawRay(rayOrigin1, Vector2.down * rayLength, Color.gray);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin1, Vector2.down, rayLength, InteractiveMask);
            if (hit)
            {
                if (hit.collider.tag == "Enemy" && hit.distance == 0)
                {
                    if (PInput.Player.IsDashing)
                    {
                        HitEnemy(hit);
                    }
                    return true;
                }
                else if (hit.collider.tag == "Trap" && hit.distance == 0)
                {
                    PInput.Player.Damage(20);
                    SetPlayerWasHitAndIsInvulnerable();
                }
            }
            return false;
        }

        public override void UpInteractiveCollision(float rayLength, int i)
        {
            Vector2 rayOrigin2 = RaycastOrigins.TopLeft;
            rayOrigin2 += Vector2.right * (VerticalRaySpacing * i);
            Debug.DrawRay(rayOrigin2, Vector2.up * rayLength, Color.blue);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin2, Vector2.up, rayLength, InteractiveMask);

            if (hit)
            {
                if (hit.collider.tag == "Enemy" && hit.distance == 0)
                {
                    if (PInput.Player.IsDashing)
                    {
                        HitEnemy(hit);
                    }
                }
                else if (hit.collider.tag == "Trap" && hit.distance == 0)
                {
                    PInput.Player.Damage(20);
                    SetPlayerWasHitAndIsInvulnerable();
                }
            }
        }
        #endregion

        private void HitEnemy(RaycastHit2D hit)
        {
            if (hit.transform.tag == "FlyingEnemy")
            {
                DestroyEnemy(hit);
            }
            EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
            enemy.DecreaseHP();
            if (enemy.Hp <= 0)
            {
                DestroyEnemy(hit);
            }
        }

        private void DestroyEnemy(RaycastHit2D hit)
        {
            _camScript.ShakeCam();
			_camScript.ChangeTargetZoom(7);
            var enemyHit = hit.collider.gameObject;
            enemyHit.layer = 10;
            enemyHit.tag = "DeadEnemy";
            PInput.Player.RecoverHp();
        }
        
        public void SetPlayerWasHitAndIsInvulnerable()
        {
            if (PInput.Player.Hp > 0)
            {
                ChangeHit();
                Invoke("ChangeHit", TimeToRecover);
                SetInvulnerability();
                Invoke("SetInvulnerability", InvulnerabilityTime);
            }
        }

        public void ChangeHit()
        {
            PInput.TakingHit = !PInput.TakingHit;
        }

        private void SetInvulnerability()
        {
            PInput.Player.IsInvulnerable = !PInput.Player.IsInvulnerable;
        }
    }
}
