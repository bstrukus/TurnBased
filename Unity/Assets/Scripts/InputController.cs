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
        public Action<KeyCode> OnKeyDown;


        #region MonoBehaviour
        private void Update()
        {
            
        }
        #endregion
    }
}
