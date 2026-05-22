using UnityEngine;

namespace Tactics
{
    // Attached to each cell GameObject at runtime.
    // Exposes the GridCell so GameManager can identify it via raycast hit.
    [RequireComponent(typeof(Collider))]
    public class CellSelector : MonoBehaviour
    {
        public GridCell Cell { get; private set; }

        public void Initialize(GridCell cell) => Cell = cell;
    }
}
