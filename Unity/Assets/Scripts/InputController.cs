﻿/*
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

            Vector3 movementVector = Vector3.zero;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                movementVector = Vector3.up;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                movementVector = Vector3.down;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                movementVector = Vector3.left;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                movementVector = Vector3.right;
            }

            if (movementVector != Vector3.zero)
            {
                MovementTriggered?.Invoke(movementVector);
            }
        }
        #endregion
    }
}
