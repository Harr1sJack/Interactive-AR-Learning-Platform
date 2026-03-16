using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private InfoPanelController infoPanel;
    [SerializeField] private ChatPanelController chatPanel;

    private Vector2 touchStartPos;
    private bool isTrackingTouch;

    private const float tapThreshold = 15f; // pixel movement allowed for tap

    private void Update()
    {
        if (chatPanel != null && chatPanel.IsOpen)
            return;
        if (AppStateManager.Instance == null ||
            AppStateManager.Instance.CurrentState != AppUIState.ModelActive)
            return;

        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        // Touch started
        if (touch.press.wasPressedThisFrame)
        {
            touchStartPos = touch.position.ReadValue();
            isTrackingTouch = true;
        }

        // Touch released
        if (touch.press.wasReleasedThisFrame && isTrackingTouch)
        {
            isTrackingTouch = false;

            Vector2 endPos = touch.position.ReadValue();
            float distance = Vector2.Distance(touchStartPos, endPos);

            // If moved too much → treat as drag
            if (distance > tapThreshold)
                return;

            int fingerId = touch.touchId.ReadValue();

            // Block if touching UI
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(fingerId))
                return;

            TrySelect(endPos);
        }
    }

    private void TrySelect(Vector2 screenPos)
    {
        if (AppStateManager.Instance.CurrentState != AppUIState.ModelActive)
            return;
        if (infoPanel.IsOpen)
            return;
        Ray ray = arCamera.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            ModelSelectable.ClearSelection();
            infoPanel.Hide();
            return;
        }

        // Universal solution (works for child colliders too)
        ModelSelectable selectable =
            hit.collider.GetComponentInParent<ModelSelectable>();

        if (selectable == null)
        {
            ModelSelectable.ClearSelection();
            infoPanel.Hide();
            return;
        }

        selectable.Select();
        if (chatPanel != null && chatPanel.IsOpen)
            chatPanel.Hide();
        infoPanel.Show(selectable.info);
    }
}