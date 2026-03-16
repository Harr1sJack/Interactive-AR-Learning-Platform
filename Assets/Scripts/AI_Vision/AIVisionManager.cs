using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class AIVisionManager : MonoBehaviour
{
    [Header("Camera")]
    public VisionCamera cameraSystem;

    [Header("UI References")]
    public GameObject scanOverlay;
    public GameObject captureButton;
    public Button menuButton;
    public GameObject resultPanel;

    public RawImage resultImage;
    public AspectRatioFitter resultAspect;

    public TMP_Text labelText;
    public TMP_Text descriptionText;
    public TMP_Text loadingText;
    public TMP_Text galleryIndicator;

    public GameObject nextButton;
    public GameObject prevButton;

    [Header("Backend")]
    public string backendURL = "https://ar-learning-backend.onrender.com/api/vision";

    private Texture2D[] galleryImages;
    private int currentIndex = 0;

    public void CaptureObject()
    {
        StartCoroutine(ProcessImage());
    }

    public void ResetScanner()
    {
        resultPanel.SetActive(false);
        scanOverlay.SetActive(true);
        captureButton.SetActive(true);

        loadingText.gameObject.SetActive(false);

        labelText.text = "";
        descriptionText.text = "";
        galleryIndicator.text = "";

        cameraSystem.ResumeCamera();
    }
    
    public void BackToMenu() 
    {
        SceneManager.LoadScene("MainMenu");    
    }

    IEnumerator ProcessImage()
    {
        loadingText.gameObject.SetActive(true);
        loadingText.text = "Scanning image...";

        captureButton.SetActive(false);

        Texture2D photo = cameraSystem.Capture();
        cameraSystem.PauseCamera();

        byte[] bytes = photo.EncodeToJPG(60);

        WWWForm form = new WWWForm();
        form.AddBinaryData("image", bytes, "capture.jpg", "image/jpeg");

        UnityWebRequest request = UnityWebRequest.Post(backendURL, form);

        request.timeout = 40;
        request.SetRequestHeader("Accept", "application/json");

        loadingText.text = "Analyzing with AI...";

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            HandleError("AI request failed. Try again.");
            yield break;
        }

        VisionResponse res;

        try
        {
            res = JsonUtility.FromJson<VisionResponse>(request.downloadHandler.text);
        }
        catch
        {
            HandleError("AI returned invalid data.");
            yield break;
        }

        // Handle unsupported content
        if (res.label == "Unsupported Content")
        {
            HandleError(res.description);
            yield break;
        }

        // Handle missing images
        if (res.images == null || res.images.Length == 0)
        {
            HandleError("No reference images found. Try another scan.");
            yield break;
        }

        labelText.text = res.label;
        descriptionText.text = res.description;

        yield return StartCoroutine(LoadGallery(res.images));

        loadingText.gameObject.SetActive(false);

        resultPanel.SetActive(true);
        scanOverlay.SetActive(false);
    }
    IEnumerator LoadGallery(string[] urls)
    {
        galleryImages = new Texture2D[urls.Length];

        for (int i = 0; i < urls.Length; i++)
        {
            string url = urls[i];

            // If it's a Base64 generated image
            if (url.StartsWith("data:image"))
            {
                string base64Data = url.Substring(url.IndexOf(",") + 1);

                byte[] imageBytes = System.Convert.FromBase64String(base64Data);

                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);

                galleryImages[i] = tex;
            }
            else
            {
                // Normal URL (Unsplash)
                UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
                req.timeout = 20;

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    galleryImages[i] = DownloadHandlerTexture.GetContent(req);
                }
            }
        }

        currentIndex = 0;
        UpdateGalleryUI();
    }

    void UpdateGalleryUI()
    {
        if (galleryImages == null || galleryImages.Length == 0)
            return;

        Texture2D tex = galleryImages[currentIndex];

        resultImage.texture = tex;

        float ratio = (float)tex.width / tex.height;
        resultAspect.aspectRatio = ratio;

        galleryIndicator.text = (currentIndex + 1) + " / " + galleryImages.Length;

        bool multiple = galleryImages.Length > 1;

        nextButton.SetActive(multiple);
        prevButton.SetActive(multiple);
    }

    public void NextImage()
    {
        if (galleryImages == null || galleryImages.Length <= 1) return;

        currentIndex++;

        if (currentIndex >= galleryImages.Length)
            currentIndex = 0;

        UpdateGalleryUI();
    }

    public void PrevImage()
    {
        if (galleryImages == null || galleryImages.Length <= 1) return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = galleryImages.Length - 1;

        UpdateGalleryUI();
    }

    void HandleError(string message)
    {
        StartCoroutine(ShowTemporaryMessage(message, 5f));

        captureButton.SetActive(true);
        cameraSystem.ResumeCamera();
    }

    IEnumerator ShowTemporaryMessage(string message, float duration)
    {
        loadingText.gameObject.SetActive(true);
        loadingText.text = message;

        yield return new WaitForSeconds(duration);

        loadingText.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class VisionResponse
{
    public string label;
    public string description;
    public string[] images;
}