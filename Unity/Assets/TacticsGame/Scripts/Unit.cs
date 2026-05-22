using UnityEngine;
using System.Collections;

namespace Tactics
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] public UnitStats stats = new UnitStats();
        [SerializeField] public int teamIndex = 0; // 0 = player, 1 = enemy

        public int CT { get; private set; } = 0;
        public GridCell CurrentCell { get; private set; }
        public bool HasMoved { get; private set; }
        public bool HasActed { get; private set; }
        public bool IsAlive => !stats.IsDead;

        private void Awake()
        {
            stats.Initialize();
        }

        public void SetInitialCT(int value) => CT = value;

        public void PlaceOnCell(GridCell cell)
        {
            if (CurrentCell != null)
                CurrentCell.Occupant = null;
            CurrentCell = cell;
            cell.Occupant = this;
            transform.position = cell.SurfacePosition + Vector3.up * 0.5f;
        }

        public void AdvanceCT() => CT += stats.speed;

        public bool IsReady => CT >= 100;

        public void StartTurn()
        {
            CT -= 100;
            HasMoved = false;
            HasActed = false;
            stats.isDefending = false;
        }

        public void SetMoved() => HasMoved = true;
        public void SetActed() => HasActed = true;

        public bool CanStillAct() => !HasActed;
        public bool CanStillMove() => !HasMoved;

        public IEnumerator AnimateMoveTo(GridCell targetCell)
        {
            Vector3 start = transform.position;
            Vector3 end = targetCell.SurfacePosition + Vector3.up * 0.5f;
            float duration = 0.25f;
            float elapsed = 0f;

            if (CurrentCell != null) CurrentCell.Occupant = null;
            CurrentCell = targetCell;
            targetCell.Occupant = this;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = end;
        }

        public IEnumerator AnimateAttack(Unit target)
        {
            Vector3 origin = transform.position;
            Vector3 toward = (target.transform.position - origin).normalized * 0.4f;
            float half = 0.12f;

            float elapsed = 0f;
            while (elapsed < half)
            {
                transform.position = Vector3.Lerp(origin, origin + toward, elapsed / half);
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < half)
            {
                transform.position = Vector3.Lerp(origin + toward, origin, elapsed / half);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = origin;
        }
    }
}
