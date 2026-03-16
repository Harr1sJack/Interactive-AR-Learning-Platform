using UnityEngine;
using UnityEngine.UI;

public class VisionCamera : MonoBehaviour
{
    public RawImage preview;
    public AspectRatioFitter aspectFitter;

    private WebCamTexture webcam;

    void Start()
    {
        webcam = new WebCamTexture();

        preview.texture = webcam;
        webcam.Play();
    }

    void Update()
    {
        if (webcam == null) return;

        float ratio = (float)webcam.width / webcam.height;

        aspectFitter.aspectRatio = ratio;

        int orientation = -webcam.videoRotationAngle;
        preview.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    public Texture2D Capture()
    {
        Texture2D photo = new Texture2D(webcam.width, webcam.height);

        photo.SetPixels(webcam.GetPixels());
        photo.Apply();

        return photo;
    }

    public void PauseCamera()
    {
        if (webcam != null && webcam.isPlaying)
        {
            webcam.Pause();
        }
    }

    public void ResumeCamera()
    {
        if (webcam != null && !webcam.isPlaying)
        {
            webcam.Play();
        }
    }
}