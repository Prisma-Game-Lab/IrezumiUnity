using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class FlyingEnemyController : MonoBehaviour
    {
        #region Variables and Structs

        Vector2 DeltaMove; // Final Vector2 with the amount of movement
        Vector2 RandomNumber; // Random factor in the movement, will be added to the x and y component of the Delta.

        public float Radious; // The area that the enemy is allowed to move randomly in. Can be seen in Editor.
        private Vector2 StartPosition; // For the area to make sense the center must be still, we're considering the start position of the Enemy.

        /// Range Structs (X and Y)
        /// Structs wwith the min and max that the movement can go in the X and Y axis.
        ///
        [System.Serializable]
        public struct XRange {

         public  float min;
         public float max;

        };
        [System.Serializable]
        public struct YRange
        {
            public float min;
            public float max;
        }
        public XRange xrange;
        public YRange yrange;

        public float Speed; // Speed that the Enemy moves inside the circle

        // The interval of time that the Enemy changes direction of movement, is also Random inside the min and max range set.
        [System.Serializable]
        public struct directionChangeTime
        {
            public float min;
            public float max;
        }

        public directionChangeTime dirChangeTime;
        private float dirChange;

        #endregion

        #region Start
        void Start()
        {
            StartPosition = transform.position;
        }
        #endregion

        #region Draw Gizmos
        //Drawing the circle that the enemy can move inside
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(StartPosition, Radious);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.08f);
        }
        #endregion

        #region Update
        // Update is called once per frame
        void Update()
        {            
            if (this.tag == "DeadEnemy")
            {
                Destroy(this.gameObject); // RIP
            }

            if (Time.time >= dirChange) // If the time is inside the window that was set 
            {
                RandomNumber = new Vector2(Random.Range(xrange.min, xrange.max), Random.Range(yrange.min, yrange.max)); // Randomize the Vector that will be added to the position
                //print(RandomNumber);
                DeltaMove.x = RandomNumber.x * Speed;
                DeltaMove.y = RandomNumber.y * Speed;

                dirChange = Time.time + Random.Range(dirChangeTime.min,dirChangeTime.max);
            }
            
            float Dist; // The Distance from the center of the circle
            Dist = Vector2.Distance(StartPosition, (Vector2)transform.position+DeltaMove);
            //print(Dist);
            if (Dist > Radious) // If the movement will cause the Enemy to go outside the circle, the movement is inverted and he goes to the opposite direction [Maybe change to random again]
                DeltaMove = -DeltaMove;

            transform.position = new Vector2(transform.position.x + DeltaMove.x, transform.position.y + DeltaMove.y);
            //transform.Translate(DeltaMove*Time.deltaTime);            
        }
        #endregion

    }
}
