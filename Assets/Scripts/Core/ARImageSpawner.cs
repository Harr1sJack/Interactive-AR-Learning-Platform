using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageSpawner : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager imageManager;
    [SerializeField] ImagePrefabMap prefabMap;
    [SerializeField] Transform spawnRoot;
    [SerializeField] private SimulationUIController simulationUI;
    [SerializeField] private ARCameraBackground cameraBackground;

    GameObject currentModel;

    private void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnImagesChanged);
    }

    private void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnImagesChanged);
    }

    public void OnImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        if (currentModel != null) return;

        foreach (var img in args.updated)
        {
            if (img.trackingState != TrackingState.Tracking)
                continue;

            var prefab = prefabMap.GetPrefab(img.referenceImage.name);
            if (prefab == null) continue;

            currentModel = Instantiate(prefab, img.transform.position, img.transform.rotation, spawnRoot);
            //currentModel = Instantiate(prefab, img.transform.position, img.transform.rotation);
            currentModel.transform.position += Vector3.up * 0.05f;

            var interaction = currentModel.AddComponent<ModelInteractionController>();
            var sim = currentModel.GetComponent<ISimulation>();
            var context = FindFirstObjectByType<ModelContextProvider>();
            context.SetContext(img.referenceImage.name);
            simulationUI.SetSimulation(sim);

            AppStateManager.Instance.SetState(AppUIState.ModelActive);
            break;
        }
    }

    public void ResetModel()
    {
        if (currentModel) Destroy(currentModel);
        currentModel = null;
        cameraBackground.enabled = true;
        AppStateManager.Instance.SetState(AppUIState.Scanning);
    }
}
