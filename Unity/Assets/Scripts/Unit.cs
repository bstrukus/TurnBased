/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;

namespace Core
{
    public class Unit : MonoBehaviour
    {
        [SerializeField]
        private Movement movement;
        public Movement Movement { get { return this.movement; } }

        private bool myTurn = false;

        #region MonoBehaviour
        private void Awake()
        {
            Debug.Assert(this.movement != null, "[Unit] Missing reference to Movement.");
        }
        #endregion

        public void StartTurn()
        {
            Debug.LogFormat("[{0}] Turn Started", gameObject.name);
            this.myTurn = true;
            this.Movement.StartTurn();

            SystemsController.Instance.Input.TurnEnd += OnTurnEnd;
        }

        public void EndTurn()
        {
            Debug.LogFormat("[{0}] Turn Ended", gameObject.name);
            this.myTurn = false;

            this.Movement.EndTurn();

            SystemsController.Instance.Turns.EndTurn(this);

            SystemsController.Instance.Input.TurnEnd -= OnTurnEnd;
        }

        private void OnTurnEnd()
        {
            EndTurn();
        }

    }
}
