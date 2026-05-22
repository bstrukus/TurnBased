using System.Collections.Generic;
using UnityEngine;

namespace Tactics
{
    public static class Pathfinder
    {
        static readonly int[] DX = { 1, -1, 0, 0 };
        static readonly int[] DZ = { 0, 0, 1, -1 };

        // BFS flood-fill — returns all cells reachable within moveBudget steps
        // respecting jump height limits. Does not include the origin cell.
        public static HashSet<GridCell> GetReachableCells(
            GridCell origin, int moveBudget, int jumpHeight, GridSystem grid)
        {
            var reachable = new HashSet<GridCell>();
            var bestRemaining = new Dictionary<GridCell, int> { [origin] = moveBudget };
            var queue = new Queue<(GridCell cell, int remaining)>();
            queue.Enqueue((origin, moveBudget));

            while (queue.Count > 0)
            {
                var (current, remaining) = queue.Dequeue();

                for (int i = 0; i < 4; i++)
                {
                    GridCell neighbor = grid.GetCell(current.X + DX[i], current.Z + DZ[i]);
                    if (neighbor == null || !neighbor.IsWalkable) continue;

                    // A cell occupied by an enemy blocks movement
                    if (neighbor.Occupant != null) continue;

                    int heightDiff = neighbor.Height - current.Height;
                    if (Mathf.Abs(heightDiff) > jumpHeight) continue;

                    // Upward movement costs extra; downward/flat costs 1
                    int cost = heightDiff > 0 ? 1 + heightDiff : 1;
                    int newRemaining = remaining - cost;
                    if (newRemaining < 0) continue;

                    if (!bestRemaining.ContainsKey(neighbor) || bestRemaining[neighbor] < newRemaining)
                    {
                        bestRemaining[neighbor] = newRemaining;
                        reachable.Add(neighbor);
                        queue.Enqueue((neighbor, newRemaining));
                    }
                }
            }

            return reachable;
        }

        // Diamond (Manhattan) attack range around origin, min to max tiles
        public static HashSet<GridCell> GetAttackableCells(
            GridCell origin, int minRange, int maxRange, GridSystem grid)
        {
            var cells = new HashSet<GridCell>();
            for (int dx = -maxRange; dx <= maxRange; dx++)
            {
                for (int dz = -maxRange; dz <= maxRange; dz++)
                {
                    int dist = Mathf.Abs(dx) + Mathf.Abs(dz);
                    if (dist < minRange || dist > maxRange) continue;
                    GridCell cell = grid.GetCell(origin.X + dx, origin.Z + dz);
                    if (cell != null) cells.Add(cell);
                }
            }
            return cells;
        }
    }
}
