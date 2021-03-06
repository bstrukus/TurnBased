﻿/*
 * Ben's TurnBased Strategy Game
 */

using System.Collections;
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

        private Coroutine moveCoroutine = null;

        private bool IsCurrentlyMoving
        {
            get { return this.currentFrame <= this.transitionFrames; }
        }

        #region MonoBehaviour
        private void Start()
        {
            // This logic should go into "start of unit's move turn", but this'll do for now
            this.originalPosition = this.transform.position;
            this.currentFrame = this.transitionFrames + 1;

            var gridRef = SystemsController.Instance.Battlefield.Grid;
            Vector3Int worldToCell = gridRef.WorldToCell(this.transform.position);
            Vector3Int[] movementOptions =
            {
                worldToCell += Vector3Int.up,
                worldToCell += Vector3Int.down,
                worldToCell += Vector3Int.right,
                worldToCell += Vector3Int.left,
            };
            Vector3 worldToLocal = gridRef.WorldToLocal(this.transform.position);
            Vector3 cellCenterWorld = gridRef.GetCellCenterWorld(worldToCell);
            string[] movementLogs =
            {
                $"Cell info of {this.gameObject.name}",
                $"WorldToCell: {worldToCell}",
                $"WorldToLocal: {worldToLocal}",
                $"CellCenterWorld: {cellCenterWorld}",
                $"UP: {gridRef.CellToWorld(movementOptions[0])}",
                $"DOWN: {gridRef.CellToWorld(movementOptions[1])}",
                $"RIGHT: {gridRef.CellToWorld(movementOptions[2])}",
                $"LEFT: {gridRef.CellToWorld(movementOptions[3])}",
            };
            Logging.PrintLines(movementLogs);
        }
        #endregion

        public static bool CanMoveToNewPosition(Vector3 origin, Vector3 newPosition, int maxMoveDistance)
        {
            Vector3Int originCellPosition = SystemsController.Instance.Battlefield.Grid.WorldToCell(origin);
            Vector3Int newCellPosition = SystemsController.Instance.Battlefield.Grid.WorldToCell(newPosition);
            Vector3Int positionDelta = newCellPosition - originCellPosition;
            int movesFromOriginalPosition = (int)Mathf.Abs(positionDelta.x) + (int)Mathf.Abs(positionDelta.y);

            string[] moveLogs = {
                $"Origin = {originCellPosition}",
                $"New = {newCellPosition}," +
                $"Delta = {positionDelta}"
            };
            Logging.PrintLines(moveLogs);

            return movesFromOriginalPosition <= maxMoveDistance;
        }

        public void StartTurn()
        {
            SystemsController.Instance.Input.MovementTriggered += OnMovementTriggered;
            SystemsController.Instance.Input.PositionReset += OnPositionReset;
        }

        public void EndTurn()
        {
            this.originalPosition = this.transform.position;

            SystemsController.Instance.Input.MovementTriggered -= OnMovementTriggered;
            SystemsController.Instance.Input.PositionReset -= OnPositionReset;
        }

        private IEnumerator Move()
        {
            for (int i = 0; i < (this.transitionFrames + 1); ++i)
            {
                float transitionDelta = (float)i / this.transitionFrames;
                this.transform.position = Vector3.Lerp(this.startPosition, this.endPosition, transitionDelta);
                yield return null;
            }
            this.moveCoroutine = null;
        }

        #region Event Handlers
        private void OnMovementTriggered(Vector3 movementVector)
        {
            if (this.moveCoroutine != null)
            {
                return;
            }

            Vector3 movementDelta = movementVector * this.moveDelta;
            this.startPosition = this.gameObject.transform.position;
            if (CanMoveToNewPosition(this.originalPosition, movementDelta + this.startPosition, this.UnitMoveDistance))
            {
                this.endPosition = this.startPosition + movementDelta;
                this.currentFrame = 0;
            }

            this.moveCoroutine = StartCoroutine(Move());
        }

        private void OnPositionReset()
        {
            this.transform.position = this.originalPosition;
        }
        #endregion  
    }
}
