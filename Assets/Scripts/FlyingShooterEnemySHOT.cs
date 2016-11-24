using UnityEngine;
using System.Collections;


namespace Assets.Scripts
{
    public class FlyingShooterEnemySHOT : MonoBehaviour {


        #region Variables
        [SerializeField]
        private bool FirstFlag; // Flag that stops the enemy from shooting 
        Vector3 PlayerLastPosition; // The position of the player wheen he hits the Ray
        public GameObject FlyingEnemyBulletPrefab; // The enemy Bullet, preferably a Prefab
        Vector3 EnemyPosition; // the position of the enemy in any given time

        // Objects that will Spawn 
        GameObject Bullet; 
        GameObject Bullet2;
        GameObject Bullet3;

        public GameObject SpawnBullet; // the bullet will Spawn in this object position
        float AngleZ; // The Angle that the player position makes with the Vector2.up. Will be used as the direction of the bullet when it Spawns


        // Self explanatory
        public float BulletSpeed; 
        public float TimeBetweenShots;
        public float Range;

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
            
            FirstFlag = true;
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
            TimeBetweenShots = 1f;
            Range = 10f;
        }
        #endregion

        #region Update
        // Update is called once per frame
        void Update() {
            //RaycastHit2D CircleHit = Physics2D.CircleCast(SpawnBullet.transform.position, Range, -Vector2.up);  TESTING 

            // raycast to test collisions with the player, to see if he's in the "Line of Sight"
            RaycastHit2D hit = Physics2D.Raycast(SpawnBullet.transform.position, -Vector2.up, Mathf.Infinity);
            RaycastHit2D hit2 = Physics2D.Raycast(SpawnBullet.transform.position, new Vector2(-1, -1), Mathf.Infinity);
            RaycastHit2D hit3 = Physics2D.Raycast(SpawnBullet.transform.position, new Vector2(1, -1), Mathf.Infinity);

            /*
            float dot = Vector2.Dot(new Vector2(-0.5f, -1), new Vector2(0.5f, -1));
            print(dot);
            float mag = Vector2.SqrMagnitude(new Vector2(-0.5f, 1)) * Vector2.SqrMagnitude(new Vector2(0.5f, -1));
            print(mag);
            float cos = dot / mag;
            print(cos);
            float Angle = Mathf.Rad2Deg*Mathf.Acos(cos);
            print(Angle);
            */

            // Vectors that are the "end" of raycasts, so we can see then using the function DrawLine
            float DF; // Degree Factor
            DF = 0.707f;
            Vector3 end = new Vector3(0, -Range, 0);
            Vector3 end2 = new Vector3(DF*-Range,DF*-Range, 0);
            Vector3 end3 = new Vector3(DF*Range,DF*-Range, 0);

            Debug.DrawLine(SpawnBullet.transform.position, SpawnBullet.transform.position + end, Color.green);
            Debug.DrawLine(SpawnBullet.transform.position, SpawnBullet.transform.position + end2, Color.green);
            Debug.DrawLine(SpawnBullet.transform.position, SpawnBullet.transform.position + end3, Color.green);

            // Every frame we falsify the Confirmation Hits Bools, to minimize false positives
            Chit1 = false;
            Chit2 = false;
            Chit3 = false;

            /* TEST CIRCLE HIOT FUNCTION
            if (CircleHit)
            {
                if (CircleHit.collider.tag == "Player")
                {
                    if (FirstFlag)
                    {
                        PlayerLastPosition = hit.collider.transform.position;
                    }
                    if (FirstFlag)// Cooldown
                    {
                        AngleZ = Vector2.Angle(Vector2.up, SpawnBullet.transform.position - PlayerLastPosition);
                        print(AngleZ);
                        Bullet = Instantiate(FlyingEnemyBulletPrefab, SpawnBullet.transform.position, Quaternion.Euler(0, 0, AngleZ)) as GameObject;
                        Bullet.gameObject.GetComponent<Arrow>().speed = -BulletSpeed;
                        FirstFlag = false;
                        Invoke("ChangeFirstFlag", TimeBetweenShots);
                    }

                }
            }
            */

            #region HITS
            //Comment only on one, the others are basically the same thing
            if (hit) // If the Raycast is colliding with smth
            {
                
                if (hit.collider.tag == "Player" && hit.distance <= Range) // If it's the Player, and the Player is is the Range previously set
                {
                    Chit1 = true;
                    //Debug.Log("SHOOT");
                    if (FirstFlag) // If the Enemy can shoot
                    {
                        PlayerLastPosition = hit.collider.transform.position; // Now the last position is where the player collided with the Ray [The center of the player only]
                        //print(PlayerLastPosition);
                    }
                    if (FirstFlag)// Cooldown
                    {
                        AngleZ = Vector2.Angle(Vector2.up,SpawnBullet.transform.position-PlayerLastPosition); // Angle that is formed between the Vector2.Up and the position of the player RELATIVE to the Enemy.
                        //print(AngleZ);
                        Bullet = Instantiate(FlyingEnemyBulletPrefab, SpawnBullet.transform.position, Quaternion.Euler(0, 0, AngleZ)) as GameObject; // Instatiate the Bullet
                        Bullet.gameObject.GetComponent<Arrow>().speed = -BulletSpeed; // Set the Speed
                        FirstFlag = false; // Can't shoot again
                        Invoke("ChangeFirstFlag", TimeBetweenShots); // Only after TimeBetweenSeconds
                    }
                }
            }
            
            if (hit2)
            {
                
                if (hit2.collider.tag == "Player" && hit.distance <= Range)
                {
                    Chit2 = true;
                    //Debug.Log("SHOOT");
                    if (FirstFlag)
                    {
                        PlayerLastPosition = hit2.collider.transform.position;
                        //print(PlayerLastPosition);
                    }
                    if (FirstFlag)// Cooldown
                    {
                        AngleZ = Vector2.Angle(Vector2.up, -SpawnBullet.transform.position + PlayerLastPosition);
                        //print(AngleZ);
                        Bullet2 = Instantiate(FlyingEnemyBulletPrefab, SpawnBullet.transform.position, Quaternion.Euler(0, 0, -AngleZ+90)) as GameObject;
                        Bullet2.gameObject.GetComponent<Arrow>().speed = -BulletSpeed;
                        FirstFlag = false;
                        Invoke("ChangeFirstFlag", TimeBetweenShots);
                    }
                }
            }
            if (hit3)
            {
               
                if (hit3.collider.tag == "Player" && hit.distance <= Range)
                {
                    Chit3 = true;
                    //Debug.Log("SHOOT");
                    if (FirstFlag)
                    {
                        PlayerLastPosition = hit3.collider.transform.position;
                        //print(PlayerLastPosition);
                    }
                    if (FirstFlag)// Cooldown
                    {
                        AngleZ = Vector2.Angle(Vector2.up, SpawnBullet.transform.position - PlayerLastPosition);
                       //print(AngleZ);
                        Bullet3 = Instantiate(FlyingEnemyBulletPrefab, SpawnBullet.transform.position, Quaternion.Euler(0, 0, AngleZ)) as GameObject;
                        Bullet3.gameObject.GetComponent<Arrow>().speed = -BulletSpeed;
                        FirstFlag = false;
                        Invoke("ChangeFirstFlag", TimeBetweenShots);
                    }
                }
            }
            #endregion

        }
        #endregion

        #region Auxiliary Functions
        // Function that is called in the Invoke, to turn the FirstFlag true so the Enemy can Shoot again
        void ChangeFirstFlag()
        {
            FirstFlag = true;
        }

        /*private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(SpawnBullet.transform.position, Range);
        }*/
        #endregion

    }
}
