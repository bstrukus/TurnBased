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

        #region MonoBehaviour
        private void Awake()
        {
            Debug.Assert(this.movement != null, "[Unit] Missing reference to Movement.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                EndTurn();
            }
        }
        #endregion

        public void StartTurn()
        {
            Debug.LogFormat("[{0}] Turn Started", gameObject.name);
        }

        public void EndTurn()
        {
            Debug.LogFormat("[{0}] Turn Ended", gameObject.name);

            this.Movement.SetCurrentPositionAsNew();

            SystemsController.Instance.Turns.EndTurn(this);
        }

    }
}
