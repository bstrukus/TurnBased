﻿/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;

namespace Core
{
    public class Movement : MonoBehaviour
    {
        public int UnitMoveDistance { get { return this.unitMovementDistance; } }
        public float MoveDelta { get { return this.moveDelta; } }

        [SerializeField]
        private float moveDelta = 1.0f;

        [SerializeField]
        private int transitionFrames = 60;

        [SerializeField]
        private int unitMovementDistance = 3;

        private Vector3 originalPosition;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private int currentFrame = 0;
        private bool myTurn = false;

        private bool IsCurrentlyMoving
        {
            get { return this.currentFrame <= this.transitionFrames; }
        }

        #region MonoBehaviour
        private void Start()
        {
            // This logic should go into "start of unit's move turn", but this'll do for now
            this.originalPosition = this.gameObject.transform.position;
            this.currentFrame = this.transitionFrames + 1;
        }

        private void Update()
        {
            if (!this.myTurn)
            {
                return;
            }

            // Check to see we are currently moving
            if (this.IsCurrentlyMoving)
            {
                float transitionDelta = (float)currentFrame / this.transitionFrames;
                this.gameObject.transform.position = Vector3.Lerp(this.startPosition, this.endPosition, transitionDelta);
                ++currentFrame;
            }
            else if (Input.GetKeyDown(KeyCode.Home))
            {
                this.gameObject.transform.position = this.originalPosition;
            }
            else
            {
                // Check to see if we've been commanded to move
                Vector3 movementDelta = QueryMovementDelta();
                this.startPosition = this.gameObject.transform.position;
                if (movementDelta != Vector3.zero && CanMoveToNewPosition(this.originalPosition, movementDelta + this.startPosition, this.UnitMoveDistance))
                {
                    this.endPosition = this.startPosition + movementDelta;
                    this.currentFrame = 0;
                }
            }
        }
        #endregion

        public static bool CanMoveToNewPosition(Vector3 origin, Vector3 newPosition, int maxMoveDistance)
        {
            Vector3 positionDelta = newPosition - origin;
            int movesFromOriginalPosition = (int)Mathf.Abs(positionDelta.x) + (int)Mathf.Abs(positionDelta.y);
            return movesFromOriginalPosition <= maxMoveDistance;
        }

        public void StartTurn()
        {
            this.myTurn = true;
        }

        public void EndTurn()
        {
            this.originalPosition = this.transform.position;

            this.myTurn = false;
        }

        private Vector3 QueryMovementDelta()
        {
            // #idea Have a base InputControler class that listens for these key events and create a new Moveable(?) class that listens to the events and responds!
            Vector3 moveVector = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                moveVector.y = this.moveDelta;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveVector.y = -this.moveDelta;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveVector.x = -this.moveDelta;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveVector.x = this.moveDelta;
            }
            return moveVector;
        }
    }
}
