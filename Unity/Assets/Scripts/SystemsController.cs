/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;

namespace Core
{
    public class SystemsController : MonoBehaviour
    {
        public static SystemsController Instance
        {
            get;
            private set;
        }

        [SerializeField]
        private InputController input;
        public InputController Input
        {
            get { return this.input; }
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
