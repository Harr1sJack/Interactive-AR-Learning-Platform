using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageSpawner : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager imageManager;
    [SerializeField] private ImagePrefabMap prefabMap;
    [SerializeField] private SimulationUIController simulationUI;
    [SerializeField] private ARCameraBackground cameraBackground;

    private GameObject currentModel;
    private ARAnchor currentAnchor;

    private bool isSpawning = false;

    private void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnImagesChanged);
    }

    private void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnImagesChanged);
    }

    private void OnImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        if (currentModel != null || isSpawning)
            return;

        foreach (var img in args.updated)
        {
            if (img.trackingState != TrackingState.Tracking)
                continue;

            StartCoroutine(SpawnAfterStabilization(img));
            break;
        }
    }

    private IEnumerator SpawnAfterStabilization(ARTrackedImage img)
    {
        isSpawning = true;

        // wait for tracking stabilization
        yield return new WaitForSeconds(0.4f);

        if (img.trackingState != TrackingState.Tracking || currentModel != null)
        {
            isSpawning = false;
            yield break;
        }

        var prefab = prefabMap.GetPrefab(img.referenceImage.name);
        if (prefab == null)
        {
            isSpawning = false;
            yield break;
        }

        // ✅ CREATE INDEPENDENT WORLD ANCHOR (NOT tied to image)
        GameObject anchorGO = new GameObject("ModelAnchor");
        anchorGO.transform.position = img.transform.position;
        anchorGO.transform.rotation = img.transform.rotation;

        currentAnchor = anchorGO.AddComponent<ARAnchor>();

        // spawn model under anchor
        currentModel = Instantiate(prefab, currentAnchor.transform);

        // local alignment (important)
        currentModel.transform.localPosition = new Vector3(0, 0.05f, 0);
        currentModel.transform.localRotation = Quaternion.identity;

        // setup interaction
        var interaction = currentModel.AddComponent<ModelInteractionController>();
        var sim = currentModel.GetComponent<ISimulation>();

        var context = FindFirstObjectByType<ModelContextProvider>();
        context.SetContext(img.referenceImage.name);

        simulationUI.SetSimulation(sim);

        AppStateManager.Instance.SetState(AppUIState.ModelActive);

        // ✅ OPTIONAL: disable image tracking to prevent jitter (especially for large models like solar system)
        imageManager.enabled = false;

        isSpawning = false;
    }

    public void ResetModel()
    {
        if (currentModel)
            Destroy(currentModel);

        if (currentAnchor)
            Destroy(currentAnchor.gameObject);

        currentModel = null;
        currentAnchor = null;
        isSpawning = false;

        // re-enable image tracking
        imageManager.enabled = true;

        cameraBackground.enabled = true;
        AppStateManager.Instance.SetState(AppUIState.Scanning);
    }
}