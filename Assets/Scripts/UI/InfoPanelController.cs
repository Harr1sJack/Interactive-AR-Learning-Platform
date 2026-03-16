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
    public bool IsOpen => isOpen;

    private void Awake()
    {
        panelRoot.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        animator.Play("Closed", 0, 1f);
    }

    public void Show(ModelInfoData info)
    {
        if (info == null)
            return;

        currentInfo = info;

        titleText.text = info.title;
        descriptionText.text = info.description;

        isOpen = true;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        AppStateManager.Instance.SetState(AppUIState.InfoOpen);

        animator.SetTrigger("Open");
    }

    public void Hide()
    {
        if (!isOpen)
            return;

        isOpen = false;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        animator.SetTrigger("Close");

        currentInfo = null;

        AppStateManager.Instance.SetState(AppUIState.ModelActive);

        ModelSelectable.ClearSelection();
    }

    public void OnCloseButtonPressed()
    {
        Hide();
    }
}