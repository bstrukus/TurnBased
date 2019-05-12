/*
 * Ben's TurnBased Strategy Game
 */

using UnityEngine;

namespace Core
{
    // Used for stuff like tables, trees, rocks, stuff you can't walk on
    public class Prop : Entity
    {
        public override bool CanPerformAction(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Attack:
                case ActionType.Move:
                case ActionType.Open:
                default:
                    return false;
            }
        }
    }
}