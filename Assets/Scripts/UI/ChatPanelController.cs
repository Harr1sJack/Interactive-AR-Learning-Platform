using UnityEngine;

public class ChatPanelController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private InfoPanelController infoPanel;

    private bool isOpen = false;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        animator.Play("Closed");
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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
        if (infoPanel != null && infoPanel.IsOpen)
            infoPanel.Hide();

        isOpen = true;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        animator.SetTrigger("Open");
    }

    public void Hide()
    {
        isOpen = false;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        animator.SetTrigger("Close");
    }
}