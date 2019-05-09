/*
 * Ben's TurnBased Strategy Game
 */

using System;
using UnityEngine;

namespace Core
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField]
        private Unit[] units;

        public Action<Unit> TurnStarted;        // Parameters(newUnit)
        public Action<Unit> TurnEnded;          // Parameters(oldUnit)
        public Action<int> NewCycleStarted;     // Parameters(newCycleNumber)

        private int currentTurn;        // Keeps track of current turn this cycle
        private int totalTurns;         // Keeps track of total turns taken thus far
        private int totalTurnCycles;    // Keeps track of total cycles (full run-through of Units)

        private Unit CurrentUnit { get { return this.units[this.currentTurn]; } }

        #region MonoBehaviour
        private void Awake()
        {
            Debug.Assert(units.Length > 0, "[TurnManager] No Units exist!");
            this.currentTurn = 0;
            this.totalTurns = 0;
            this.totalTurnCycles = 0;
        }

        private void Start()
        {
            StartTurn();
        }
        #endregion

        public void EndTurn(Unit unit)
        {
            TurnEnded?.Invoke(this.CurrentUnit);
            NextTurn();
        }

        private void StartTurn()
        {
            this.CurrentUnit.StartTurn();
            TurnStarted?.Invoke(this.CurrentUnit);
        }

        private void NextTurn()
        {
            Debug.Log("[TurnManager] NextTurn : Called");
            ++this.currentTurn;
            ++this.totalTurns;
            if (this.currentTurn >= units.Length)
            {
                this.currentTurn = 0;
                ++this.totalTurnCycles;

                NewCycleStarted?.Invoke(this.totalTurnCycles);
                Debug.Log("[TurnManager] NextTurn : New cycle started");
            }

            StartTurn();
        }
    }
}
