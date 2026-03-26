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

    private const float tapThreshold = 15f;

    private void Update()
    {
        // Block interaction if chat is open
        if (chatPanel != null && chatPanel.IsOpen)
            return;

        // FIX: Use IsModelInteractive so selection still works when the
        // info panel is open (InfoOpen state). Previously the strict
        // ModelActive check meant you couldn't tap a different part while
        // the panel was showing.
        if (AppStateManager.Instance == null ||
            !AppStateManager.Instance.IsModelInteractive)
            return;

        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        // Touch start
        if (touch.press.wasPressedThisFrame)
        {
            touchStartPos = touch.position.ReadValue();
            isTrackingTouch = true;
        }

        // Touch release
        if (touch.press.wasReleasedThisFrame && isTrackingTouch)
        {
            isTrackingTouch = false;

            Vector2 endPos = touch.position.ReadValue();
            float distance = Vector2.Distance(touchStartPos, endPos);

            // Ignore drag
            if (distance > tapThreshold)
                return;

            int fingerId = touch.touchId.ReadValue();

            // Ignore UI touches
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(fingerId))
                return;

            TrySelect(endPos);
        }
    }

    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            ModelSelectable.ClearSelection();
            infoPanel.Hide();
            return;
        }

        // Works for child colliders
        ModelSelectable selectable =
            hit.collider.GetComponentInParent<ModelSelectable>();

        if (selectable == null)
        {
            ModelSelectable.ClearSelection();
            infoPanel.Hide();
            return;
        }

        // Select + highlight handled internally
        selectable.Select();

        // Close chat if open
        if (chatPanel != null && chatPanel.IsOpen)
            chatPanel.Hide();

        // Always update panel (no blocking)
        infoPanel.Show(selectable.info);
    }
}