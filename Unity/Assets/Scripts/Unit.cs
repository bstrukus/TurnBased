/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;

namespace Core
{
    public class Unit : MonoBehaviour
    {
        [SerializeField]
        private KeyCode nextTurnKey = KeyCode.Return;

        [SerializeField]
        private Movement movement;
        public Movement Movement { get { return this.movement; } }

        private bool myTurn = false;
        private bool oneFramePassed = false;    // #hack This is so that our turn won't end as soon as it begins due to the nextTurnKey being "triggered" from the previous turn

        #region MonoBehaviour
        private void Awake()
        {
            Debug.Assert(this.movement != null, "[Unit] Missing reference to Movement.");
        }

        private void Update()
        {
            if (!this.myTurn)
            {
                this.oneFramePassed = false;
                return;
            }

            if (oneFramePassed && Input.GetKeyUp(this.nextTurnKey))
            {
                EndTurn();
            }
            this.oneFramePassed = true;
        }
        #endregion

        public void StartTurn()
        {
            Debug.LogFormat("[{0}] Turn Started", gameObject.name);
            this.myTurn = true;
            this.Movement.StartTurn();
        }

        public void EndTurn()
        {
            Debug.LogFormat("[{0}] Turn Ended", gameObject.name);
            this.myTurn = false;

            this.Movement.EndTurn();

            SystemsController.Instance.Turns.EndTurn(this);
        }

    }
}
