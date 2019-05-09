/*
 * Ben's TurnBased Strategy Game
 */

using System.Collections;
using UnityEngine;

namespace Core
{
    public class Unit : MonoBehaviour
    {
        [SerializeField]
        private Movement movement;
        public Movement Movement { get { return this.movement; } }

        private Coroutine attackCoroutine = null;

        #region MonoBehaviour
        private void Awake()
        {
            Debug.Assert(this.movement != null, "[Unit] Missing reference to Movement.");
        }
        #endregion

        public void StartTurn()
        {
            Debug.LogFormat("[{0}] Turn Started", gameObject.name);
            this.Movement.StartTurn();

            SystemsController.Instance.Input.TurnEnd += OnTurnEnd;
            SystemsController.Instance.Input.ActionTriggered += OnActionTriggered;
        }

        public void EndTurn()
        {
            Debug.LogFormat("[{0}] Turn Ended", gameObject.name);
            this.Movement.EndTurn();

            SystemsController.Instance.Turns.EndTurn(this);

            SystemsController.Instance.Input.TurnEnd -= OnTurnEnd;
            SystemsController.Instance.Input.ActionTriggered -= OnActionTriggered;
        }

        private IEnumerator Attack()
        {
            int steps = 15;
            float fullRotation = 360.0f;
            float rotationChunks = fullRotation / (float)steps;

            for (int i = 0; i < steps; ++i)
            {
                this.transform.Rotate(Vector3.forward, rotationChunks);
                yield return null;
            }
            this.attackCoroutine = null;
        }

        #region Event Handlers
        private void OnTurnEnd()
        {
            EndTurn();
        }

        private void OnActionTriggered()
        {
            Debug.Log($"[Unit] Action triggered for '{this.gameObject.name}'");
            if (this.attackCoroutine == null)
            {
                this.attackCoroutine = StartCoroutine(Attack());
            }
        }
        #endregion
    }
}
