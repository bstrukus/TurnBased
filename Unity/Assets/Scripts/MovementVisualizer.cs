/*
 * Ben's TurnBased Strategy Game
 */

using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class MovementVisualizer : MonoBehaviour
    {
        private Movement currentActor;
        public Movement CurrentActor
        {
            get { return this.currentActor; }
            set
            {
                this.currentActor = value;
                UpdateVisuals();
            }
        }

        [SerializeField]
        private GameObject possibleMovementMarker = null;

        [SerializeField]
        private Vector3 movementOffset = Vector3.zero;

        private List<GameObject> currentMarkers = null;

        #region MonoBehaviour
        private void Awake()
        {
            currentMarkers = new List<GameObject>();
        }

        private void Start()
        {
            SystemsController.Instance.Turns.TurnStarted += OnTurnStarted;

            float cellHeight = SystemsController.Instance.Battlefield.Grid.cellSize.y;
            movementOffset = new Vector3(0, cellHeight / 2.0f);
        }
        #endregion

        #region Event Handlers
        private void OnTurnStarted(Unit unit)
        {
            this.CurrentActor = unit.GetComponent<Movement>();
            ClearExistingMoveMarkers();
            UpdateVisuals();
        }
        #endregion

        [ContextMenu("Recalculate visuals")]
        private void RecalcVisuals()
        {
            ClearExistingMoveMarkers();
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            int moveDistance = this.CurrentActor.UnitMoveDistance;
            Vector3[] axes = { SystemsController.Instance.Battlefield.RightLeft,
                               SystemsController.Instance.Battlefield.UpDown };
            Vector3 origin = this.CurrentActor.transform.position - this.movementOffset;
            for (int i = -moveDistance; i <= moveDistance; ++i)
            {
                for (int j = -moveDistance; j <= moveDistance; ++j)
                {
                    Vector3 newPosition = origin + (axes[0] * this.CurrentActor.MoveDelta * (float)i)
                                                 + (axes[1] * this.CurrentActor.MoveDelta * (float)j);

                    if (Movement.CanMoveToNewPosition(origin, newPosition, moveDistance))
                    {
                        GameObject moveMarker = GameObject.Instantiate(possibleMovementMarker, newPosition, Quaternion.identity, this.transform);
                        currentMarkers.Add(moveMarker);
                    }
                }
            }
        }

        private void ClearExistingMoveMarkers()
        {
            foreach (var moveMarker in currentMarkers)
            {
                GameObject.Destroy(moveMarker);
            }
            currentMarkers.Clear();
        }
    }
}
