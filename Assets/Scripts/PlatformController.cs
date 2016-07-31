using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlatformController : WaypointController
    {
        #region Variables
        public LayerMask PassengerMask;
        private List<PassengerMovement> _passengerMovement;
        private Dictionary<Transform, Controller2D> _passengerDictionary;
        #endregion

        #region Start
        public new void Start()
        {
            base.Start();
            _passengerDictionary = new Dictionary<Transform, Controller2D>();
        }
        #endregion

        #region Update
        public new void Update()
        {
            base.Update ();
            Vector2 velocity = CalculateWaypointMovement ();
            CalculatePassengerMovement(velocity);

            MovePassengers(true);
            transform.Translate(velocity);
            MovePassengers(false);
        }

        private void CalculatePassengerMovement(Vector2 velocity)
        {
            var movedPassengers = new HashSet<Transform>();
            _passengerMovement = new List<PassengerMovement>();

            var directionX = Mathf.Sign(velocity.x);
            var directionY = Mathf.Sign(velocity.y);
            
            AddVerticalPlatformMovement(velocity, directionY, movedPassengers);

            AddHorizontalPlatformMovement(velocity, directionX, movedPassengers);

            AddHorizontalOrDonwardPlatformMovementForPlayerOnTop(velocity, directionY, movedPassengers);
        }

        /// <summary>
        /// Vertically moving platform
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="directionY"></param>
        /// <param name="movedPassengers"></param>
        private void AddVerticalPlatformMovement(Vector2 velocity, float directionY, HashSet<Transform> movedPassengers)
        {
            if (velocity.y != 0)
            {
                var rayLength = Mathf.Abs(velocity.y) + SkinWidth;

                for (var i = 0; i < VerticalRayCount; i++)
                {
                    /*store rays starting point : if we are going left then get topleft ...*/
                    Vector2 rayOrigin = (directionY == -1) ? RaycastOrigins.BottomLeft : RaycastOrigins.TopLeft;
                    rayOrigin += Vector2.right * (VerticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, PassengerMask);

                    /*player hit something and is not inside the obstacle*/
                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            var pushX = (directionY == 1) ? velocity.x : 0;
                            var pushY = velocity.y - (hit.distance - SkinWidth) * directionY;

                            _passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY),
                                directionY == 1, true));
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Horizontally moving platform
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="directionX"></param>
        /// <param name="movedPassengers"></param>
        private void AddHorizontalPlatformMovement(Vector2 velocity, float directionX, HashSet<Transform> movedPassengers)
        {
            if (velocity.x != 0)
            {
                var rayLength = Mathf.Abs(velocity.x) + SkinWidth;

                for (int i = 0; i < HorizontalRayCount; i++)
                {
                    Vector2 rayOrigin = (directionX == -1) ? RaycastOrigins.BottomLeft : RaycastOrigins.BottomRight;
                    rayOrigin += Vector2.up*(HorizontalRaySpacing*i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right*directionX, rayLength, PassengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            var pushX = velocity.x - (hit.distance - SkinWidth)*directionX;
                            var pushY = -SkinWidth;

                            _passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), false, true));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Passenger on top of a horizontally or downward moving platform
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="directionY"></param>
        /// <param name="movedPassengers"></param>
        private void AddHorizontalOrDonwardPlatformMovementForPlayerOnTop(Vector2 velocity, float directionY,
            HashSet<Transform> movedPassengers)
        {
            if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                var rayLength = SkinWidth * 2;

                for (var i = 0; i < VerticalRayCount; i++)
                {
                    Vector2 rayOrigin = RaycastOrigins.TopLeft + Vector2.right * (VerticalRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, PassengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            var pushX = velocity.x;
                            var pushY = velocity.y;

                            _passengerMovement.Add(new PassengerMovement(hit.transform, new Vector2(pushX, pushY), true, false));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Move Passengers using passenger dictionary
        /// </summary>
        /// <param name="beforeMovePlatform"></param>
        private void MovePassengers(bool beforeMovePlatform)
        {
            foreach (PassengerMovement passenger in _passengerMovement)
            {
                if (!_passengerDictionary.ContainsKey(passenger.Transform))
                    _passengerDictionary.Add(passenger.Transform, passenger.Transform.GetComponent<Controller2D>());

                if (passenger.MoveBeforePlatform == beforeMovePlatform)
                    _passengerDictionary[passenger.Transform].Move(passenger.Velocity, passenger.StandingOnPlatform);
            }
        }
        #endregion

        #region Structs
        struct PassengerMovement
        {
            public readonly Transform Transform;
            public readonly Vector2 Velocity;
            public readonly bool StandingOnPlatform;
            public readonly bool MoveBeforePlatform;

            public PassengerMovement(Transform transform, Vector2 velocity, bool standingOnPlatform, bool moveBeforePlatform)
            {
                Transform = transform;
                Velocity = velocity;
                StandingOnPlatform = standingOnPlatform;
                MoveBeforePlatform = moveBeforePlatform;
            }
        }

        #endregion

    }
}