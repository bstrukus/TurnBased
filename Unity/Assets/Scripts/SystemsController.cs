/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;

namespace Core
{
    public class SystemsController : MonoBehaviour
    {
        public static SystemsController Instance { get; private set; }

        #region Game Systems
        [SerializeField]
        private InputController input;
        public InputController Input { get { return this.input; } }

        [SerializeField]
        private MovementVisualizer movementVisualizer;
        public MovementVisualizer MovementVisualizer { get { return this.movementVisualizer; } }

        [SerializeField]
        private TurnManager turns;
        public TurnManager Turns { get { return this.turns; } }

        [SerializeField]
        private Battlefield battleField;
        public Battlefield Battlefield { get { return this.battleField; } }

        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            Instance = this;

            Debug.Assert(this.input != null, "[SystemsController] Missing reference to InputController.");
            Debug.Assert(this.turns != null, "[SystemsController] Missing reference to TurnManager.");
            Debug.Assert(this.movementVisualizer != null, "[SystemsController] Missing reference to MovementVisualizer.");
            Debug.Assert(this.battleField != null, "[SystemsController] Missing reference to Battlefield.");
        }
        #endregion
    }
}
