using UnityEngine;

public class BrainSimulation : MonoBehaviour, ISimulation
{
    [SerializeField] private float rotationSpeed = 10f;

    private bool isRunning = true;
    private float currentSpeedMultiplier = 1f;
    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isRunning) return;

        transform.Rotate(Vector3.up * rotationSpeed * currentSpeedMultiplier * Time.deltaTime, Space.World);
    }

    public void Play() => isRunning = true;

    public void Pause() => isRunning = false;

    public void SpeedUp() => currentSpeedMultiplier *= 1.2f;

    public void SlowDown() => currentSpeedMultiplier *= 0.8f;

    public void ResetSimulation()
    {
        currentSpeedMultiplier = 1f;
        transform.rotation = initialRotation;
    }
}