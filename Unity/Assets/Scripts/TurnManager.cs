using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactics
{
    public class TurnManager : MonoBehaviour
    {
        public event Action<Unit> TurnStarted;
        public event Action<Unit> TurnEnded;

        public Unit ActiveUnit { get; private set; }

        private List<Unit> units = new List<Unit>();

        public void RegisterUnits(List<Unit> allUnits)
        {
            units = new List<Unit>(allUnits);

            // Stagger starting CT so units spread out on the first cycle
            for (int i = 0; i < units.Count; i++)
                units[i].SetInitialCT(i * (100 / Mathf.Max(1, units.Count)));
        }

        // Advance time until the next unit is ready, then start its turn.
        public void AdvanceToNextTurn()
        {
            if (ActiveUnit != null)
            {
                TurnEnded?.Invoke(ActiveUnit);
                ActiveUnit = null;
            }

            RemoveDeadUnits();

            while (ActiveUnit == null)
            {
                Unit candidate = GetHighestReadyUnit();
                if (candidate != null)
                {
                    ActiveUnit = candidate;
                    ActiveUnit.StartTurn();
                    TurnStarted?.Invoke(ActiveUnit);
                }
                else
                {
                    foreach (var u in units) u.AdvanceCT();
                }
            }
        }

        // Returns a list simulating upcoming turn order (for UI display).
        // Does NOT modify actual CT values.
        public List<Unit> PredictTurnOrder(int count = 6)
        {
            var simCT = new Dictionary<Unit, int>();
            foreach (var u in units) simCT[u] = u.CT;

            var order = new List<Unit>();
            int safety = 0;
            while (order.Count < count && safety++ < 500)
            {
                Unit best = null;
                foreach (var u in units)
                {
                    if (simCT[u] >= 100)
                    {
                        if (best == null || simCT[u] > simCT[best]) best = u;
                    }
                }
                if (best != null)
                {
                    order.Add(best);
                    simCT[best] -= 100;
                }
                else
                {
                    foreach (var u in units) simCT[u] += u.stats.speed;
                }
            }
            return order;
        }

        public List<Unit> GetAllUnits() => new List<Unit>(units);

        public bool IsPlayerTurn => ActiveUnit != null && ActiveUnit.teamIndex == 0;

        private Unit GetHighestReadyUnit()
        {
            Unit best = null;
            foreach (var u in units)
            {
                if (!u.IsReady || !u.IsAlive) continue;
                if (best == null || u.CT > best.CT) best = u;
            }
            return best;
        }

        private void RemoveDeadUnits()
        {
            units.RemoveAll(u => u == null || !u.IsAlive);
        }
    }
}
