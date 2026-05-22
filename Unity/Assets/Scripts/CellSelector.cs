using UnityEngine;

namespace Tactics
{
    // Attached to each cell GameObject at runtime; forwards OnMouseDown to GameManager.
    [RequireComponent(typeof(Collider))]
    public class CellSelector : MonoBehaviour
    {
        private GridCell cell;

        public void Initialize(GridCell c) => cell = c;

        private void OnMouseDown()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCellClicked(cell);
        }
    }
}
