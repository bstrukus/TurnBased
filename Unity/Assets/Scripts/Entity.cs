/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;

namespace Core
{
    public enum ActionType
    {
        Move,
        Attack,
        Open
    }

    public abstract class Entity : MonoBehaviour
    {
        public abstract bool CanPerformAction(ActionType actionType);
    }
}