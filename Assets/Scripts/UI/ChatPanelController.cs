using UnityEngine;

public class ChatPanelController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InfoPanelController infoPanel;
    [SerializeField] private ChatbotController chatbotController;

    private bool isOpen = false;
    private bool isTransitioning = false;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        // Same fix as InfoPanelController: don't force-play Closed here.
        // Just zero the CanvasGroup and let the Animator's default state own it.
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
    }

    public void Toggle()
    {
        if (isOpen)
            Hide();
        else
            Show();
    }

    public void Show()
    {
        // FIX: Guard against spam during transition window.
        if (isTransitioning)
            return;

        // FIX: Force-hide info panel regardless of its isOpen state,
        // removing dependency on isOpen being in sync.
        if (infoPanel != null)
            infoPanel.Hide();

        isOpen = true;
        isTransitioning = true;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        animator.SetTrigger("Open");

        CancelInvoke(nameof(ClearTransition));
        Invoke(nameof(ClearTransition), 0.15f);
    }

    public void Hide()
    {
        if (isTransitioning)
            return;

        isOpen = false;
        isTransitioning = true;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        animator.SetTrigger("Close");

        // FIX: Stop any running typewriter and unlock input.
        if (chatbotController != null)
            chatbotController.OnPanelClosed();

        CancelInvoke(nameof(ClearTransition));
        Invoke(nameof(ClearTransition), 0.15f);
    }

    private void ClearTransition()
    {
        isTransitioning = false;
    }
}