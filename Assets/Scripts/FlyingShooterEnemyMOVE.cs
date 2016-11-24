using UnityEngine;
using System.Collections;
using Assets.Scripts;

namespace Assets.Scripts
{
    [RequireComponent(typeof(WaypointController))]
    public class FlyingShooterEnemyMOVE : MonoBehaviour
    {


        public float maxXRange; //maximum range in which the the enemy moves in the X axys
        public float minXRange; //minimum range in which the the enemy moves in the X axys
        public float maxYRange; //maximum range in which the the enemy moves in the Y axys
        public float minYRange; //minimum range in which the the enemy moves in the Y axys
        public float maxTimeRange; //maximum time range in which the enemy stays in the same direction
        public float minTimeRange; //minimum time range in which the enemy stays in the same direction
        public float moveSpeed; //enemy's moving speed


        private float randomX; //random number pulled between the minXRange and maxXRange
        private float randomY; //random number pulled between the minYRange and maxYRange
        private int directionX = 1; //enemy's direction in the X axys (changes from 1 to -1)
        private int directionY = 1; //enemy's direction in the Y axys (changes from 1 to -1)
        private float dirChange; //time frame in which the direction of the enemy will change randomly

        void Update()
        {

            //change to random direction at random intervals within a set time interval set by the GDs
            if (Time.time >= dirChange)
            {
                //these are the variables randomX and randomY in action
                randomX = Random.Range(minXRange, maxXRange);
                randomY = Random.Range(minYRange, maxYRange);

                //since I didn't find better anyway to change it between 1 and -1, I used this technical resource
                if (directionX == 1)
                {
                    directionX = -1;
                }
                else
                {
                    directionX = 1;
                }

                if (directionY == 1)
                {
                    directionY = -1;
                }
                else
                {
                    directionX = 1;
                }

                dirChange = Time.time + Random.Range(minTimeRange, maxTimeRange);
            }

            // this is in case the the enemy reaches the boundaries of its aggro range, in this case, the direction will have to change in order for it to not
            // travel past its determined range
            /*
            if (transform.Translate. += directionX > maxXRange)
            {
                transform.position.x = transform.position.x + directionX;
            }

            if (minXRange > transform.position.x -= directionX)
            {
                transform.position.x += directionX;
            }

            if (transform.position.y += directionY > maxYRange)
            {
                transform.position.y -= directionY;
            }

            if (minYRange > transform.position.x -= directionY)
            {
                transform.position.y += directionY;
            }
            */

            //formulas to make the enemy move as is expected (well, kinda expected since it moves randomly) within the parameters
            Vector2 MoveX;
            transform.Translate(new Vector2(randomX, randomY) * moveSpeed * Time.deltaTime * directionX);
            transform.Translate(new Vector2(randomX, randomY) * moveSpeed * Time.deltaTime * directionY);


            // this makes sure the position is inside the borders(look, it was in the site where I got this method, so I'm not changing it)
            //transform.position.x = Mathf.Clamp(transform.position.x, minXRange, maxXRange);
            //transform.position.y = Mathf.Clamp(transform.position.y, minYRange, maxYRange);
        }

    }
}