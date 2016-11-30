using UnityEngine;
using System.Collections;


namespace Assets.Scripts
{
    public class FlyingShooterEnemySHOT : MonoBehaviour {


        #region Variables

        public bool DrawGizmos;
        [SerializeField]
        Vector3 PlayerLastPosition; // The position of the player wheen he hits the Ray
        public GameObject FlyingEnemyBulletPrefab; // The enemy Bullet, preferably a Prefab
        Vector3 EnemyPosition; // the position of the enemy in any given time

        // Objects that will Spawn 
        GameObject Bullet;
        public GameObject SpawnBullet; // the bullet will Spawn in this object position
        float AngleZ; // The Angle that the player relative position makes with the Vector3.up. Will be used as the direction of the bullet when it Spawns


        // Self explanatory
        public float BulletSpeed;
        public float Range;
        public float Cooldown;
        private float CooldownTime;

        // Bools that are TRUE when the player is hitting the Raycasts
        [SerializeField]
        private bool Chit1;
        [SerializeField]
        private bool Chit2;
        [SerializeField]
        private bool Chit3;

        #endregion

        #region Start
        // Use this for initialization
        void Start() {
            EnemyPosition = transform.position;
            SetDefault();
            Chit1 = false;
            Chit2 = false;
            Chit3 = false;        

        }
        #endregion


        /// TURN OF THIS FUNCTION IF TESTING
        ///  Set the Default values to the enemy, if this function is active you can't change anything in the Editor
        ///
        #region SetDefault
        void SetDefault()
        {
            BulletSpeed = 4f;
            Cooldown = 1f;
            Range = 10f;
        }
        #endregion

        #region Update
        // Update is called once per frame
        void Update() {
            

            // Overlap Circle hit to test collisions with the player, to see if he's in the "Line of Sight"/"Shooting Area"
            Collider2D []CircleHit = Physics2D.OverlapCircleAll(SpawnBullet.transform.position,Range,1 << LayerMask.NameToLayer("Player"));

            // Vectors that are the "end" of raycasts, so we can see then using the function DrawLine
            float DF; // Degree Factor
            DF = 0.707f;
            Vector3 end2 = new Vector3(DF*-Range,DF*-Range, 0);
            Vector3 end3 = new Vector3(DF*Range,DF*-Range, 0);

            if (DrawGizmos)
            {
                Debug.DrawLine(SpawnBullet.transform.position, SpawnBullet.transform.position + end2, Color.green);
                Debug.DrawLine(SpawnBullet.transform.position, SpawnBullet.transform.position + end3, Color.green);
            }


            #region CircleHit              
            if (CircleHit.Length > 0)
           {
                for (int i = 0; i < CircleHit.Length; i++)
                {
                    if (CircleHit[i].tag == "Player")
                    {
                        PlayerLastPosition = CircleHit[i].transform.position;
                        Vector3 RelativePosition = SpawnBullet.transform.position - PlayerLastPosition;
                        AngleZ = Vector3.Angle(-Vector3.up, RelativePosition);
                        //print(AngleZ);


                        //SHOOT
                        if (AngleZ >= 135)
                        {
                            Debug.DrawLine(SpawnBullet.transform.position, PlayerLastPosition, Color.red); // Draw a line o sight 

                            if (Time.time >= CooldownTime && AngleZ >= 135)
                            {
                                Bullet = Instantiate(FlyingEnemyBulletPrefab, SpawnBullet.transform.position, Quaternion.identity) as GameObject; // Instatiate the Bullet
                                Bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, RelativePosition); // Set the rotation of the bullet
                                Bullet.gameObject.GetComponent<Arrow>().speed = -BulletSpeed; // Set the Speed
                                CooldownTime = Time.time + Cooldown; // Set next Cooldown
                            }
                        }
                    } 
                }
                
            }
            #endregion
        }
        #endregion

        #region DrawGizmos
        private void OnDrawGizmos()
        {
            if (DrawGizmos)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(SpawnBullet.transform.position, Range);
            }
        }
        #endregion

        #region Auxiliary Functions    
        #endregion

    }
}
