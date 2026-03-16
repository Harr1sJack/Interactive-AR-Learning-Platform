using UnityEngine;

public enum AppUIState
{
    Scanning,
    ModelActive,
    InfoOpen
}

public class AppStateManager : MonoBehaviour
{
    public static AppStateManager Instance { get; private set; }

    public AppUIState CurrentState { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject scanIndicator;
    [SerializeField] private GameObject controlButtons;
    [SerializeField] private GameObject infoPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetState(AppUIState.Scanning);
    }

    public void SetState(AppUIState state)
    {
        CurrentState = state;
        Debug.Log("UI State: " + state);

        scanIndicator.SetActive(state == AppUIState.Scanning);
        controlButtons.SetActive(state == AppUIState.ModelActive);
    }
}