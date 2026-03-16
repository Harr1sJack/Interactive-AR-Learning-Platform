using UnityEngine;

public class WaterCycleSimulation : MonoBehaviour, ISimulation
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [SerializeField] private float baseSpeed = 1f;

    private float speedMultiplier = 1f;
    private bool isRunning = true;

    void Update()
    {
        if (animator == null) return;

        animator.speed = isRunning ? baseSpeed * speedMultiplier : 0f;
    }

    public void Play()
    {
        isRunning = true;
    }

    public void Pause()
    {
        isRunning = false;
    }

    public void SpeedUp()
    {
        speedMultiplier *= 1.2f;
        ClampSpeed();
    }

    public void SlowDown()
    {
        speedMultiplier *= 0.8f;
        ClampSpeed();
    }

    public void ResetSimulation()
    {
        speedMultiplier = 1f;

        if (animator != null)
        {
            animator.Play(0, 0, 0f);   // restart animation
            animator.Update(0f);       // force update immediately
        }
    }

    private void ClampSpeed()
    {
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.2f, 5f);
    }
}