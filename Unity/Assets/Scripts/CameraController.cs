using UnityEngine;

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

        private float orbitYaw = 225f;          // initial angle: camera south-west of grid
        private float targetOrbitYaw = 225f;
        private float freeYaw;
        private float freePitch;
        private Vector3 lastMousePos;

        private void Start()
        {
            // Sync free-mode angles to the current camera orientation
            freeYaw   = transform.eulerAngles.y;
            freePitch = transform.eulerAngles.x;
            ApplyOrbit(snap: true);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
                ToggleMode();

            if (CurrentMode == Mode.Orbit) UpdateOrbit();
            else UpdateFree();
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

        private void UpdateOrbit()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Q)) targetOrbitYaw -= 90f;
            if (UnityEngine.Input.GetKeyDown(KeyCode.E)) targetOrbitYaw += 90f;

            orbitYaw = Mathf.LerpAngle(orbitYaw, targetOrbitYaw,
                Time.deltaTime * orbitRotateSpeed);

            orbitDistance -= UnityEngine.Input.mouseScrollDelta.y * scrollSpeed;
            orbitDistance = Mathf.Clamp(orbitDistance, 8f, 45f);

            ApplyOrbit(snap: false);
        }

        private void ApplyOrbit(bool snap)
        {
            float yaw   = snap ? targetOrbitYaw : orbitYaw;
            Quaternion rot = Quaternion.Euler(orbitPitch, yaw, 0f);
            transform.position = focusPoint + rot * new Vector3(0f, 0f, -orbitDistance);
            transform.LookAt(focusPoint);
        }

        // ── Free ─────────────────────────────────────────────────────────────────

        private void UpdateFree()
        {
            // Right-mouse drag → look around
            if (UnityEngine.Input.GetMouseButtonDown(1))
                lastMousePos = UnityEngine.Input.mousePosition;

            if (UnityEngine.Input.GetMouseButton(1))
            {
                Vector3 delta = UnityEngine.Input.mousePosition - lastMousePos;
                freeYaw   += delta.x * freeLookSpeed * Time.deltaTime;
                freePitch -= delta.y * freeLookSpeed * Time.deltaTime;
                freePitch  = Mathf.Clamp(freePitch, -80f, 80f);
                lastMousePos = UnityEngine.Input.mousePosition;
            }

            transform.rotation = Quaternion.Euler(freePitch, freeYaw, 0f);

            // WASD pan in the camera's horizontal plane
            float h = UnityEngine.Input.GetAxis("Horizontal");
            float v = UnityEngine.Input.GetAxis("Vertical");
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 right   = Vector3.ProjectOnPlane(transform.right,   Vector3.up).normalized;
            transform.position += (forward * v + right * h) * panSpeed * Time.deltaTime;

            // Scroll = move forward/back
            float scroll = UnityEngine.Input.mouseScrollDelta.y;
            transform.position += transform.forward * scroll * scrollSpeed;

            // Update focus point to follow free camera (keeps orbit re-entry sane)
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
