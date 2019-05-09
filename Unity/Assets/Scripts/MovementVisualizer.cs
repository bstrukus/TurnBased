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

        private List<GameObject> currentMarkers = null;

        #region MonoBehaviour
        private void Awake()
        {
            currentMarkers = new List<GameObject>();
        }

        private void Start()
        {
            SystemsController.Instance.Turns.TurnStarted += OnTurnStarted;
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

        private void UpdateVisuals()
        {
            int moveDistance = this.CurrentActor.UnitMoveDistance;
            Vector3 origin = this.CurrentActor.transform.position;
            for (int i = -moveDistance; i <= moveDistance; ++i)
            {
                for (int j = -moveDistance; j <= moveDistance; ++j)
                {
                    Vector3 newPosition = origin + new Vector3(this.CurrentActor.MoveDelta * (float)i,
                                                               this.CurrentActor.MoveDelta * (float)j, -1.0f);
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
