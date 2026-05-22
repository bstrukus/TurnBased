using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Tactics
{
    // Simple screen-space action menu shown during a unit's turn.
    // Attach to a Canvas GameObject. Buttons are auto-created on Awake if
    // not assigned in the Inspector.
    public class ActionMenuUI : MonoBehaviour
    {
        [Header("Optional - assign in Inspector or auto-created")]
        [SerializeField] private Button moveButton;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button itemButton;
        [SerializeField] private Button defendButton;
        [SerializeField] private Button endTurnButton;

        [Header("Turn-order panel")]
        [SerializeField] private Text turnOrderText;

        private Canvas canvas;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            if (canvas == null)
                canvas = gameObject.AddComponent<Canvas>();

            // Ensure these are always present regardless of how the Canvas was created
            if (GetComponent<CanvasScaler>() == null)
                gameObject.AddComponent<CanvasScaler>();
            if (GetComponent<GraphicRaycaster>() == null)
                gameObject.AddComponent<GraphicRaycaster>();

            // Always enforce screen-space overlay — if the Canvas was added via
            // the Editor it may have been left on a different render mode.
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            // Buttons need an EventSystem in the scene to receive clicks.
            // Create one automatically if the scene doesn't already have it.
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<InputSystemUIInputModule>();
            }

            // Anchor is bottom-centre (0.5, 0), pivot is bottom-centre (0.5, 0).
            // Positive Y = pixels above the bottom edge of the screen.
            if (moveButton    == null) moveButton    = CreateButton("Move",     new Vector2(-165, 60));
            if (attackButton  == null) attackButton  = CreateButton("Attack",   new Vector2( -55, 60));
            if (itemButton    == null) itemButton    = CreateButton("Item",     new Vector2(  55, 60));
            if (defendButton  == null) defendButton  = CreateButton("Defend",   new Vector2( 165, 60));
            if (endTurnButton == null) endTurnButton = CreateButton("End Turn", new Vector2(   0, 10));

            if (turnOrderText == null)
            {
                var go = new GameObject("TurnOrderText");
                go.transform.SetParent(transform, false);
                turnOrderText = go.AddComponent<Text>();
                turnOrderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                turnOrderText.fontSize = 14;
                turnOrderText.color = Color.white;
                turnOrderText.alignment = TextAnchor.UpperLeft;
                var rt = go.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(10, -10);
                rt.sizeDelta = new Vector2(200, 160);
            }

            moveButton.onClick.AddListener(()    => GameManager.Instance?.OnActionSelected(ActionChoice.Move));
            attackButton.onClick.AddListener(()  => GameManager.Instance?.OnActionSelected(ActionChoice.Attack));
            itemButton.onClick.AddListener(()    => GameManager.Instance?.OnActionSelected(ActionChoice.Item));
            defendButton.onClick.AddListener(()  => GameManager.Instance?.OnActionSelected(ActionChoice.Defend));
            endTurnButton.onClick.AddListener(() => GameManager.Instance?.OnActionSelected(ActionChoice.EndTurn));

            Hide();
        }

        public void Show(Unit unit)
        {
            canvas.enabled = true;
            moveButton.interactable   = unit.CanStillMove();
            attackButton.interactable = unit.CanStillAct();
            itemButton.interactable   = unit.CanStillAct() && unit.stats.itemCount > 0;
            defendButton.interactable = unit.CanStillAct();
        }

        public void Hide() => canvas.enabled = false;

        public void UpdateTurnOrder(System.Collections.Generic.List<Unit> order)
        {
            if (turnOrderText == null) return;
            var sb = new System.Text.StringBuilder("Turn Order:\n");
            for (int i = 0; i < order.Count; i++)
                sb.AppendLine($"  {i + 1}. {order[i].stats.unitName} (CT:{order[i].CT})");
            turnOrderText.text = sb.ToString();
        }

        private Button CreateButton(string label, Vector2 anchoredPos)
        {
            var go = new GameObject(label + "Button");
            go.transform.SetParent(transform, false);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

            var btn = go.AddComponent<Button>();

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(go.transform, false);
            var txt = textGo.AddComponent<Text>();
            txt.text = label;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 16;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            var txtRt = textGo.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(100, 40);
            rt.anchoredPosition = anchoredPos;

            return btn;
        }
    }

    public enum ActionChoice { Move, Attack, Item, Defend, EndTurn }
}
