using UnityEngine;
using System.Collections;


namespace Assets.Scripts
{
    public class FlyingShooterEnemySHOT : MonoBehaviour {

        [SerializeField]
        private bool FirstFlag;
        Vector3 PlayerLastPosition;
        public GameObject FlyingEnemyBulletPrefab;
        Vector3 EnemyPosition;

        GameObject Bullet;
        GameObject Bullet2;
        GameObject Bullet3;
        public GameObject SpawnBullet;
        float AngleZ;
        
        public float BulletSpeed;
        public float TimeBetweenShots;
        
        

        // Use this for initialization
         void Start() {
            
            FirstFlag = true;
            EnemyPosition = transform.position;
            SetDefault();        

        }

        void ChangeFirstFlag()
        {
            FirstFlag = true;
        }
        void SetDefault()
        {
            BulletSpeed = 4f;
            TimeBetweenShots = 1f;
        }

        // Update is called once per frame
        void FixedUpdate() {
            RaycastHit2D hit = Physics2D.Raycast(EnemyPosition, -Vector2.up, 8);
            RaycastHit2D hit2 = Physics2D.Raycast(EnemyPosition, new Vector2(-1, -1), 8);
            RaycastHit2D hit3 = Physics2D.Raycast(EnemyPosition, new Vector2(1, -1), 8);

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

            Vector3 end = new Vector3(0, -8, 0);
            Vector3 end2 = new Vector3(-8, -8, 0);
            Vector3 end3 = new Vector3(8, -8, 0);

            Debug.DrawLine(EnemyPosition, EnemyPosition + end, Color.green);
            Debug.DrawLine(EnemyPosition, EnemyPosition + end2, Color.green);
            Debug.DrawLine(EnemyPosition, EnemyPosition + end3, Color.green);

            if (hit == true)
            {
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("SHOOT");
                    if (FirstFlag)
                    {
                        PlayerLastPosition = hit.collider.transform.position;
                        print(PlayerLastPosition);
                    }
                    if (FirstFlag)// Cooldown
                    {
                        AngleZ = Vector2.Angle(Vector2.up,SpawnBullet.transform.position-PlayerLastPosition);
                        print(AngleZ);
                        Bullet = Instantiate(FlyingEnemyBulletPrefab, SpawnBullet.transform.position, Quaternion.Euler(0, 0, AngleZ)) as GameObject;
                        Bullet.gameObject.GetComponent<Arrow>().speed = -BulletSpeed;
                        FirstFlag = false;
                        Invoke("ChangeFirstFlag", TimeBetweenShots);
                    }
                }
            }
            if (hit2 == true)
            {
                if (hit2.collider.tag == "Player")
                {
                    Debug.Log("SHOOT");
                    if (FirstFlag)
                    {
                        PlayerLastPosition = hit2.collider.transform.position;
                        print(PlayerLastPosition);
                    }
                    if (FirstFlag)// Cooldown
                    {
                        AngleZ = Vector2.Angle(Vector2.up, -SpawnBullet.transform.position + PlayerLastPosition);
                        print(AngleZ);
                        Bullet2 = Instantiate(FlyingEnemyBulletPrefab, SpawnBullet.transform.position, Quaternion.Euler(0, 0, -AngleZ+90)) as GameObject;
                        Bullet2.gameObject.GetComponent<Arrow>().speed = -BulletSpeed;
                        FirstFlag = false;
                        Invoke("ChangeFirstFlag", TimeBetweenShots);
                    }
                }
            }
            if (hit3 == true)
            {
                if (hit3.collider.tag == "Player")
                {
                    Debug.Log("SHOOT");
                    if (FirstFlag)
                    {
                        PlayerLastPosition = hit3.collider.transform.position;
                        print(PlayerLastPosition);
                    }
                    if (FirstFlag)// Cooldown
                    {
                        AngleZ = Vector2.Angle(Vector2.up, SpawnBullet.transform.position - PlayerLastPosition);
                        print(AngleZ);
                        Bullet3 = Instantiate(FlyingEnemyBulletPrefab, SpawnBullet.transform.position, Quaternion.Euler(0, 0, AngleZ)) as GameObject;
                        Bullet3.gameObject.GetComponent<Arrow>().speed = -BulletSpeed;
                        FirstFlag = false;
                        Invoke("ChangeFirstFlag", TimeBetweenShots);
                    }
                }
            }

        }

    }
}
