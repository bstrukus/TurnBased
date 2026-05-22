using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tactics
{
    public enum GameState
    {
        Setup,
        SelectingAction,
        SelectingMoveTarget,
        SelectingAttackTarget,
        SelectingItemTarget,
        Animating,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Scene references")]
        [SerializeField] private GridSystem gridSystem;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private ActionMenuUI actionMenu;
        [SerializeField] private CameraController cameraController;

        [Header("Unit prefabs")]
        [SerializeField] private GameObject unitPrefab;

        [Header("Team setup (edit in Inspector)")]
        [SerializeField] private GameObject team0Parent;
        [SerializeField] private GameObject team1Parent;

        [SerializeField] private UnitSetup[] team0Units;
        [SerializeField] private UnitSetup[] team1Units;

        public GameState State { get; private set; } = GameState.Setup;

        private HashSet<GridCell> highlightedCells = new HashSet<GridCell>();

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var allUnits = new List<Unit>();
            allUnits.AddRange(SpawnTeam(team0Units, 0));
            allUnits.AddRange(SpawnTeam(team1Units, 1));

            turnManager.RegisterUnits(allUnits);
            turnManager.TurnStarted += OnTurnStarted;
            turnManager.TurnEnded   += OnTurnEnded;

            // Kick off first turn
            turnManager.AdvanceToNextTurn();
        }

        // ── Spawning ─────────────────────────────────────────────────────────────

        private List<Unit> SpawnTeam(UnitSetup[] setups, int team)
        {
            var spawned = new List<Unit>();
            if (setups == null) return spawned;

            foreach (var setup in setups)
            {
                GridCell cell = gridSystem.GetCell(setup.startX, setup.startZ);
                if (cell == null || cell.Occupant != null) continue;

                GameObject go = unitPrefab != null
                    ? Instantiate(unitPrefab)
                    : CreateDefaultUnitObject(setup.stats.unitName, team);

                var unit = go.GetComponent<Unit>() ?? go.AddComponent<Unit>();
                unit.stats = setup.stats;
                unit.teamIndex = team;
                unit.PlaceOnCell(cell);
                spawned.Add(unit);
            }
            return spawned;
        }

        private GameObject CreateDefaultUnitObject(string unitName, int team)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = unitName;
            go.transform.SetParent(team == 0 ? team0Parent.transform : team1Parent.transform);
            go.GetComponent<Renderer>().material.color = team == 0
                ? new Color(0.2f, 0.4f, 1.0f)
                : new Color(1.0f, 0.2f, 0.2f);
            return go;
        }

        // ── Turn events ──────────────────────────────────────────────────────────

        private void OnTurnStarted(Unit unit)
        {
            gridSystem.ClearAllHighlights();
            gridSystem.SetHighlight(unit.CurrentCell, CellHighlight.Selected);
            RefreshTurnOrderUI();

            if (unit.teamIndex == 0)
            {
                // Player turn: show action menu
                EnterState(GameState.SelectingAction);
                actionMenu.Show(unit);

                if (cameraController != null)
                    cameraController.SetFocus(unit.CurrentCell.SurfacePosition);
            }
            else
            {
                // Enemy/AI turn: run simple AI
                StartCoroutine(RunAITurn(unit));
            }
        }

        private void OnTurnEnded(Unit unit)
        {
            gridSystem.ClearAllHighlights();
            actionMenu.Hide();
        }

        // ── Player input ─────────────────────────────────────────────────────────

        public void OnCellClicked(GridCell cell)
        {
            if (State == GameState.Animating) return;
            Unit active = turnManager.ActiveUnit;
            if (active == null) return;

            switch (State)
            {
                case GameState.SelectingMoveTarget:
                    if (highlightedCells.Contains(cell))
                        StartCoroutine(ExecuteMove(active, cell));
                    break;

                case GameState.SelectingAttackTarget:
                    if (highlightedCells.Contains(cell) && cell.Occupant != null
                        && cell.Occupant.teamIndex != active.teamIndex)
                        StartCoroutine(ExecuteAttack(active, cell.Occupant));
                    break;

                case GameState.SelectingItemTarget:
                    if (highlightedCells.Contains(cell) && cell.Occupant != null
                        && cell.Occupant.teamIndex == active.teamIndex)
                        StartCoroutine(ExecuteItem(active, cell.Occupant));
                    break;
            }
        }

        public void OnActionSelected(ActionChoice choice)
        {
            if (State != GameState.SelectingAction) return;
            Unit active = turnManager.ActiveUnit;
            if (active == null) return;

            switch (choice)
            {
                case ActionChoice.Move:
                    if (active.CanStillMove()) BeginMoveSelection(active);
                    break;
                case ActionChoice.Attack:
                    if (active.CanStillAct()) BeginAttackSelection(active);
                    break;
                case ActionChoice.Item:
                    if (active.CanStillAct() && active.stats.itemCount > 0) BeginItemSelection(active);
                    break;
                case ActionChoice.Defend:
                    if (active.CanStillAct()) ExecuteDefend(active);
                    break;
                case ActionChoice.EndTurn:
                    EndPlayerTurn();
                    break;
            }
        }

        // Cancel target-selection and return to the action menu
        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse    = Mouse.current;

            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
                CancelTargetSelection();

            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                HandleClickRaycast(mouse.position.ReadValue());
        }

        private void HandleClickRaycast(Vector2 screenPos)
        {
            if (Camera.main == null) return;
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var selector = hit.collider.GetComponent<CellSelector>();
                if (selector != null) OnCellClicked(selector.Cell);
            }
        }

        private void CancelTargetSelection()
        {
            if (State == GameState.SelectingMoveTarget
                || State == GameState.SelectingAttackTarget
                || State == GameState.SelectingItemTarget)
            {
                ClearHighlights();
                EnterState(GameState.SelectingAction);
                actionMenu.Show(turnManager.ActiveUnit);
            }
        }

        // ── Action: Move ─────────────────────────────────────────────────────────

        private void BeginMoveSelection(Unit unit)
        {
            actionMenu.Hide();
            ClearHighlights();
            var reachable = Pathfinder.GetReachableCells(
                unit.CurrentCell, unit.stats.move, unit.stats.jump, gridSystem);
            ShowHighlights(reachable, CellHighlight.Move);
            EnterState(GameState.SelectingMoveTarget);
        }

        private IEnumerator ExecuteMove(Unit unit, GridCell target)
        {
            EnterState(GameState.Animating);
            ClearHighlights();
            yield return StartCoroutine(unit.AnimateMoveTo(target));
            unit.SetMoved();
            gridSystem.SetHighlight(unit.CurrentCell, CellHighlight.Selected);

            if (unit.CanStillAct())
            {
                EnterState(GameState.SelectingAction);
                actionMenu.Show(unit);
            }
            else
            {
                EndPlayerTurn();
            }
        }

        // ── Action: Attack ────────────────────────────────────────────────────────

        private void BeginAttackSelection(Unit unit)
        {
            actionMenu.Hide();
            ClearHighlights();
            var range = Pathfinder.GetAttackableCells(
                unit.CurrentCell, 1, unit.stats.attackRange, gridSystem);
            // Only highlight cells that actually have an enemy on them
            var enemies = new HashSet<GridCell>();
            foreach (var c in range)
                if (c.Occupant != null && c.Occupant.teamIndex != unit.teamIndex)
                    enemies.Add(c);
            ShowHighlights(enemies, CellHighlight.Attack);
            EnterState(GameState.SelectingAttackTarget);
        }

        private IEnumerator ExecuteAttack(Unit attacker, Unit target)
        {
            EnterState(GameState.Animating);
            ClearHighlights();

            yield return StartCoroutine(attacker.AnimateAttack(target));

            int damage = target.stats.TakeDamage(attacker.stats.attack);
            Debug.Log($"{attacker.stats.unitName} hits {target.stats.unitName} for {damage} damage. " +
                      $"({target.stats.currentHP}/{target.stats.maxHP} HP remaining)");

            if (target.stats.IsDead)
            {
                Debug.Log($"{target.stats.unitName} was defeated!");
                target.CurrentCell.Occupant = null;
                Destroy(target.gameObject);
            }

            attacker.SetActed();
            attacker.SetMoved(); // Attacking uses your turn entirely (FFT convention)
            gridSystem.SetHighlight(attacker.CurrentCell, CellHighlight.Selected);
            EndPlayerTurn();
        }

        // ── Action: Item ──────────────────────────────────────────────────────────

        private void BeginItemSelection(Unit unit)
        {
            actionMenu.Hide();
            ClearHighlights();
            // Items can target self + adjacent allies
            var range = Pathfinder.GetAttackableCells(unit.CurrentCell, 0, 1, gridSystem);
            range.Add(unit.CurrentCell);
            var allies = new HashSet<GridCell>();
            foreach (var c in range)
                if (c.Occupant != null && c.Occupant.teamIndex == unit.teamIndex
                    && c.Occupant.stats.currentHP < c.Occupant.stats.maxHP)
                    allies.Add(c);
            ShowHighlights(allies, CellHighlight.Move);
            EnterState(GameState.SelectingItemTarget);
        }

        private IEnumerator ExecuteItem(Unit user, Unit target)
        {
            EnterState(GameState.Animating);
            ClearHighlights();
            yield return new WaitForSeconds(0.3f);

            int healed = target.stats.Heal(user.stats.itemHealAmount);
            user.stats.itemCount--;
            Debug.Log($"{user.stats.unitName} uses item on {target.stats.unitName}. " +
                      $"Healed {healed} HP. ({target.stats.currentHP}/{target.stats.maxHP})");

            user.SetActed();
            user.SetMoved();
            gridSystem.SetHighlight(user.CurrentCell, CellHighlight.Selected);
            EndPlayerTurn();
        }

        // ── Action: Defend ────────────────────────────────────────────────────────

        private void ExecuteDefend(Unit unit)
        {
            unit.stats.isDefending = true;
            unit.SetActed();
            unit.SetMoved();
            Debug.Log($"{unit.stats.unitName} defends. DEF doubled until next turn.");
            EndPlayerTurn();
        }

        // ── End turn ──────────────────────────────────────────────────────────────

        private void EndPlayerTurn()
        {
            ClearHighlights();
            actionMenu.Hide();
            EnterState(GameState.Animating);
            CheckWinCondition();
            if (State != GameState.GameOver)
                turnManager.AdvanceToNextTurn();
        }

        // ── AI turn ───────────────────────────────────────────────────────────────

        private IEnumerator RunAITurn(Unit unit)
        {
            EnterState(GameState.Animating);
            yield return new WaitForSeconds(0.6f);

            // Try to attack a nearby player unit
            var attackRange = Pathfinder.GetAttackableCells(
                unit.CurrentCell, 1, unit.stats.attackRange, gridSystem);
            Unit attackTarget = null;
            foreach (var c in attackRange)
                if (c.Occupant != null && c.Occupant.teamIndex != unit.teamIndex)
                    { attackTarget = c.Occupant; break; }

            if (attackTarget != null)
            {
                yield return StartCoroutine(unit.AnimateAttack(attackTarget));
                int dmg = attackTarget.stats.TakeDamage(unit.stats.attack);
                Debug.Log($"[AI] {unit.stats.unitName} hits {attackTarget.stats.unitName} for {dmg}.");
                if (attackTarget.stats.IsDead)
                {
                    attackTarget.CurrentCell.Occupant = null;
                    Destroy(attackTarget.gameObject);
                }
            }
            else
            {
                // Move toward nearest enemy
                var reachable = Pathfinder.GetReachableCells(
                    unit.CurrentCell, unit.stats.move, unit.stats.jump, gridSystem);
                GridCell bestCell = null;
                float bestDist = float.MaxValue;

                foreach (var u in turnManager.GetAllUnits())
                {
                    if (u.teamIndex == unit.teamIndex) continue;
                    foreach (var c in reachable)
                    {
                        float d = Vector3.Distance(c.SurfacePosition, u.CurrentCell.SurfacePosition);
                        if (d < bestDist) { bestDist = d; bestCell = c; }
                    }
                }

                if (bestCell != null)
                    yield return StartCoroutine(unit.AnimateMoveTo(bestCell));
            }

            yield return new WaitForSeconds(0.3f);
            CheckWinCondition();
            if (State != GameState.GameOver)
                turnManager.AdvanceToNextTurn();
        }

        // ── Win condition ─────────────────────────────────────────────────────────

        private void CheckWinCondition()
        {
            bool team0Alive = false, team1Alive = false;
            foreach (var u in turnManager.GetAllUnits())
            {
                if (u.teamIndex == 0) team0Alive = true;
                if (u.teamIndex == 1) team1Alive = true;
            }

            if (!team1Alive) { Debug.Log("=== PLAYER WINS ==="); EnterState(GameState.GameOver); }
            else if (!team0Alive) { Debug.Log("=== ENEMY WINS ==="); EnterState(GameState.GameOver); }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void EnterState(GameState newState)
        {
            State = newState;
        }

        private void ShowHighlights(HashSet<GridCell> cells, CellHighlight type)
        {
            highlightedCells = cells;
            gridSystem.SetHighlights(cells, type);
        }

        private void ClearHighlights()
        {
            gridSystem.ClearAllHighlights();
            if (turnManager.ActiveUnit?.CurrentCell != null)
                gridSystem.SetHighlight(turnManager.ActiveUnit.CurrentCell, CellHighlight.Selected);
            highlightedCells.Clear();
        }

        private void RefreshTurnOrderUI()
        {
            if (actionMenu != null)
                actionMenu.UpdateTurnOrder(turnManager.PredictTurnOrder(6));
        }
    }

    // ── Inspector data class ──────────────────────────────────────────────────────

    [System.Serializable]
    public class UnitSetup
    {
        public UnitStats stats = new UnitStats();
        public int startX = 0;
        public int startZ = 0;
    }
}
