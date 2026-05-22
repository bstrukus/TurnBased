using UnityEngine;
using UnityEngine.InputSystem;

namespace Tactics
{
    // Two modes:
    //   Orbit  — fixed-distance isometric, Q/E rotate 90° around the grid centre
    //   Free   — WASD pan, right-mouse drag orbit, scroll zoom
    // Tab toggles between them.
    public class CameraController : MonoBehaviour
    {
        public enum Mode { Orbit, Free }

        [Header("Shared")]
        [SerializeField] private Vector3 focusPoint = new Vector3(10f, 0f, 10f);
        [SerializeField] private float scrollSpeed = 3f;

        [Header("Orbit mode")]
        [SerializeField] private float orbitDistance = 22f;
        [SerializeField] private float orbitPitch = 40f;    // degrees above horizon
        [SerializeField] private float orbitRotateSpeed = 8f;

        [Header("Free mode")]
        [SerializeField] private float panSpeed = 12f;
        [SerializeField] private float freeLookSpeed = 90f; // deg/sec while dragging

        public Mode CurrentMode { get; private set; } = Mode.Orbit;

        private float orbitYaw = 225f;      // initial angle: camera south-west of grid
        private float targetOrbitYaw = 225f;
        private float freeYaw;
        private float freePitch;
        private Vector2 lastMousePos;

        private void Start()
        {
            freeYaw   = transform.eulerAngles.y;
            freePitch = transform.eulerAngles.x;
            ApplyOrbit(snap: true);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse    = Mouse.current;
            if (keyboard == null || mouse == null) return;

            if (keyboard.tabKey.wasPressedThisFrame)
                ToggleMode();

            if (CurrentMode == Mode.Orbit) UpdateOrbit(keyboard, mouse);
            else                           UpdateFree(keyboard, mouse);
        }

        private void ToggleMode()
        {
            CurrentMode = CurrentMode == Mode.Orbit ? Mode.Free : Mode.Orbit;
            if (CurrentMode == Mode.Free)
            {
                freeYaw   = transform.eulerAngles.y;
                freePitch = transform.eulerAngles.x;
            }
        }

        // ── Orbit ────────────────────────────────────────────────────────────────

        private void UpdateOrbit(Keyboard kb, Mouse mouse)
        {
            if (kb.qKey.wasPressedThisFrame) targetOrbitYaw -= 90f;
            if (kb.eKey.wasPressedThisFrame) targetOrbitYaw += 90f;

            orbitYaw = Mathf.LerpAngle(orbitYaw, targetOrbitYaw,
                Time.deltaTime * orbitRotateSpeed);

            orbitDistance -= mouse.scroll.ReadValue().y * scrollSpeed;
            orbitDistance  = Mathf.Clamp(orbitDistance, 8f, 45f);

            ApplyOrbit(snap: false);
        }

        private void ApplyOrbit(bool snap)
        {
            float      yaw = snap ? targetOrbitYaw : orbitYaw;
            Quaternion rot = Quaternion.Euler(orbitPitch, yaw, 0f);
            transform.position = focusPoint + rot * new Vector3(0f, 0f, -orbitDistance);
            transform.LookAt(focusPoint);
        }

        // ── Free ─────────────────────────────────────────────────────────────────

        private void UpdateFree(Keyboard kb, Mouse mouse)
        {
            // Right-mouse drag → look around
            if (mouse.rightButton.wasPressedThisFrame)
                lastMousePos = mouse.position.ReadValue();

            if (mouse.rightButton.isPressed)
            {
                Vector2 delta = mouse.position.ReadValue() - lastMousePos;
                freeYaw   += delta.x * freeLookSpeed * Time.deltaTime;
                freePitch -= delta.y * freeLookSpeed * Time.deltaTime;
                freePitch  = Mathf.Clamp(freePitch, -80f, 80f);
                lastMousePos = mouse.position.ReadValue();
            }

            transform.rotation = Quaternion.Euler(freePitch, freeYaw, 0f);

            // WASD pan in the camera's horizontal plane
            float h = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
            float v = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 right   = Vector3.ProjectOnPlane(transform.right,   Vector3.up).normalized;
            transform.position += (forward * v + right * h) * panSpeed * Time.deltaTime;

            // Scroll → dolly
            transform.position += transform.forward * mouse.scroll.ReadValue().y * scrollSpeed;

            // Keep focus point in sync for clean orbit re-entry
            focusPoint = transform.position + transform.forward * orbitDistance;
        }

        // External: snap focus to a world position (e.g. when turn changes)
        public void SetFocus(Vector3 point)
        {
            focusPoint = point;
            if (CurrentMode == Mode.Orbit) ApplyOrbit(snap: true);
        }
    }
}
