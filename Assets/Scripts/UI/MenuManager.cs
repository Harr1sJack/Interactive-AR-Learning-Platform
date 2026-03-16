using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OpenAIVision()
    {
        SceneManager.LoadScene("AI_Vision");
    }

    public void OpenImageTracking()
    {
        SceneManager.LoadScene("ARMain");
    }

    public void OpenPlaneTracking()
    {
        SceneManager.LoadScene("AR_PlaneTracking");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}