using TMPro;
using UnityEngine;

public class InfoPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Animator animator;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private ModelInfoData currentInfo;
    private bool isOpen;
    private bool isTransitioning;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        panelRoot.SetActive(true);

        // FIX: Removed animator.Play("Closed", 0, 1f).
        // Forcing normalizedTime=1f in Awake leaves the Animator in an
        // unsettled "just finished" position. The very next SetTrigger("Open")
        // gets consumed but produces no visible transition — the state machine
        // hasn't committed to Any State yet. Instead just zero the CanvasGroup
        // here and let the Animator's own default Closed state own everything.
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show(ModelInfoData info)
    {
        if (info == null)
            return;

        // FIX: Ignore calls during the 0.10s animation window.
        // Rapid taps were firing multiple "Open" triggers in quick succession,
        // which corrupts the Animator state machine.
        if (isTransitioning)
            return;

        currentInfo = info;
        titleText.text = info.title;
        descriptionText.text = info.description;

        isOpen = true;
        isTransitioning = true;

        // Make visible before the animation plays so SlideIn has something to show.
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        AppStateManager.Instance.SetState(AppUIState.InfoOpen);
        animator.SetTrigger("Open");

        // Clear guard after animation duration + small buffer.
        CancelInvoke(nameof(ClearTransition));
        Invoke(nameof(ClearTransition), 0.15f);
    }

    public void Hide()
    {
        // FIX: Removed the early-return guard "if (!isOpen) return" that was
        // causing the panel to silently ignore Hide() when isOpen drifted out
        // of sync (e.g. after the chat panel opened and tried to hide it).
        // Now we always execute a full reset when Hide() is called.
        if (isTransitioning)
            return;

        isOpen = false;
        isTransitioning = true;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        animator.SetTrigger("Close");

        currentInfo = null;

        AppStateManager.Instance.SetState(AppUIState.ModelActive);
        ModelSelectable.ClearSelection();

        CancelInvoke(nameof(ClearTransition));
        Invoke(nameof(ClearTransition), 0.15f);
    }

    private void ClearTransition()
    {
        isTransitioning = false;
    }

    public void OnCloseButtonPressed()
    {
        Hide();
    }
}