using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ModelInteractionController : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.2f;

    [Header("Scaling")]
    [SerializeField] private float scaleSpeed = 0.005f;
    [SerializeField] private float minRelativeScale = 0.3f;   // 8x smaller
    [SerializeField] private float maxRelativeScale = 2f;     // 2x bigger

    private float initialPinchDistance;
    private Vector3 baseScale;
    private bool baseScaleInitialized = false;

    private void Start()
    {
        baseScale = transform.localScale;
        baseScaleInitialized = true;
    }

    private void Update()
    {
        if (AppStateManager.Instance == null ||
            AppStateManager.Instance.CurrentState != AppUIState.ModelActive)
            return;

        if (Touchscreen.current == null || !baseScaleInitialized)
            return;

        var touches = Touchscreen.current.touches;

        int activeTouches = 0;
        foreach (var t in touches)
        {
            if (t.press.isPressed)
                activeTouches++;
        }

        // =========================
        // ONE FINGER → ROTATE
        // =========================
        if (activeTouches == 1)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (!touch.press.isPressed)
                return;

            int fingerId = touch.touchId.ReadValue();

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(fingerId))
                return;

            Vector2 delta = touch.delta.ReadValue();

            if (delta.sqrMagnitude < 1f)
                return;

            float rotationY = -delta.x * rotationSpeed;
            float rotationX = delta.y * rotationSpeed;

            // Y rotation around WORLD UP (stable horizontal spin)
            transform.Rotate(Vector3.up, rotationY, Space.World);

            // X rotation around CAMERA RIGHT (natural tilt)
            Vector3 cameraRight = Camera.main.transform.right;
            transform.Rotate(cameraRight, rotationX, Space.World);
        }

        // =========================
        // TWO FINGERS → SCALE
        // =========================
        else if (activeTouches >= 2)
        {
            var touch0 = touches[0];
            var touch1 = touches[1];

            if (!touch0.press.isPressed || !touch1.press.isPressed)
                return;

            int finger0 = touch0.touchId.ReadValue();
            int finger1 = touch1.touchId.ReadValue();

            // Ignore if either finger is on UI
            if (EventSystem.current != null &&
                (EventSystem.current.IsPointerOverGameObject(finger0) ||
                 EventSystem.current.IsPointerOverGameObject(finger1)))
                return;

            float currentDistance = Vector2.Distance(
                touch0.position.ReadValue(),
                touch1.position.ReadValue());

            if (initialPinchDistance == 0)
            {
                initialPinchDistance = currentDistance;
                return;
            }

            float pinchDelta = currentDistance - initialPinchDistance;

            // Current relative scale (relative to base scale)
            float currentRelative = transform.localScale.x / baseScale.x;

            float newRelative = currentRelative + (pinchDelta * scaleSpeed);

            newRelative = Mathf.Clamp(newRelative, minRelativeScale, maxRelativeScale);

            transform.localScale = baseScale * newRelative;

            initialPinchDistance = currentDistance;
        }
        else
        {
            initialPinchDistance = 0;
        }
    }
}