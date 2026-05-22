using UnityEngine;

namespace Tactics
{
    public class GridCell
    {
        public int X { get; }
        public int Z { get; }
        public int Height { get; set; }
        public bool IsWalkable { get; set; } = true;
        public Unit Occupant { get; set; }
        public GameObject Visual { get; set; }

        // Top surface world position (units stand here)
        public Vector3 SurfacePosition => new Vector3(X, Height, Z);

        public GridCell(int x, int z, int height = 0)
        {
            X = x;
            Z = z;
            Height = height;
        }
    }
}
