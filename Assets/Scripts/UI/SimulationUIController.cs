using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SimulationUIController : MonoBehaviour
{
    private ISimulation currentSimulation;

    [SerializeField] private ARCameraBackground cameraBackground;
    [SerializeField] private InfoPanelController infoPanel;   // 👈 Add this
    [SerializeField] private ChatPanelController chatPanel;

    public void SetSimulation(ISimulation simulation)
    {
        currentSimulation = simulation;
    }

    public void Play()
    {
        currentSimulation?.Play();
    }

    public void Pause()
    {
        currentSimulation?.Pause();
    }

    public void SpeedUp()
    {
        currentSimulation?.SpeedUp();
    }

    public void SlowDown()
    {
        currentSimulation?.SlowDown();
    }

    public void ResetSimulation()
    {
        currentSimulation?.ResetSimulation();

        if (infoPanel != null)
            infoPanel.Hide();

        if (chatPanel != null)
            chatPanel.Hide();

        ModelSelectable.ClearSelection();
    }

    public void ToggleCameraBG()
    {
        cameraBackground.enabled = !cameraBackground.enabled;
    }

    public void BackToMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }
}