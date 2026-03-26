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

        // FIX: Keep control buttons visible in BOTH ModelActive AND InfoOpen.
        // Previously, InfoOpen caused controlButtons to hide (false), which
        // made the UI disappear and froze all interaction.
        controlButtons.SetActive(state == AppUIState.ModelActive || state == AppUIState.InfoOpen);
    }

    /// <summary>
    /// Returns true when the model is placed and interactive —
    /// covers both normal interaction and when the info panel is open.
    /// Use this in place of (CurrentState == ModelActive) checks in
    /// ModelInteractionController and SelectionController.
    /// </summary>
    public bool IsModelInteractive =>
        CurrentState == AppUIState.ModelActive ||
        CurrentState == AppUIState.InfoOpen;
}