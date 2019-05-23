/*
 * Ben's TurnBased Strategy Game
 */

using System;
using UnityEngine;

namespace Core
{
    // #todo Make this class handle more input, right now it adds nothing to the input solution
    public class InputController : MonoBehaviour
    {
        public Action<Vector3> MovementTriggered;
        public Action TurnEnd;
        public Action PositionReset;
        public Action ActionTriggered;


        #region MonoBehaviour
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                TurnEnd?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Home))
            {
                PositionReset?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                ActionTriggered?.Invoke();
            }

            Vector3 movementVector = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                movementVector = SystemsController.Instance.Battlefield.UpDown;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                movementVector = -SystemsController.Instance.Battlefield.UpDown;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                movementVector = -SystemsController.Instance.Battlefield.RightLeft;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                movementVector = SystemsController.Instance.Battlefield.RightLeft;
            }

            if (movementVector != Vector3.zero)
            {
                MovementTriggered?.Invoke(movementVector);
            }
        }
        #endregion
    }
}
