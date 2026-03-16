using UnityEngine;

public class SolarSystemSimulation : MonoBehaviour, ISimulation
{
    [System.Serializable]
    public class Orbit
    {
        public Transform center;
        public float speed;
    }

    public Transform sun;
    public float sunSpeed = 10f;
    public Orbit[] planets;

    float speedMultiplier = 1f;
    bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;

        sun.Rotate(Vector3.up * sunSpeed * speedMultiplier * Time.deltaTime);

        foreach (var p in planets)
            p.center.Rotate(Vector3.up * p.speed * speedMultiplier * Time.deltaTime);
    }

    public void Play() => isRunning = true;
    public void Pause() => isRunning = false;
    public void SpeedUp() => speedMultiplier *= 1.2f;
    public void SlowDown() => speedMultiplier *= 0.8f;
    public void ResetSimulation() => speedMultiplier = 1f;
}
