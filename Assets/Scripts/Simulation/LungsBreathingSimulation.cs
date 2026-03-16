using UnityEngine;

public class LungsBreathingSimulation : MonoBehaviour, ISimulation
{
    [SerializeField] private Animator animator;
    [SerializeField] private float baseBreathingSpeed = 1f;

    private float speedMultiplier = 1f;
    private bool isRunning = true;

    private float animationTime = 0f;

    private void Update()
    {
        if (animator == null || !isRunning)
            return;

        float speed = baseBreathingSpeed * speedMultiplier;

        animationTime += Time.deltaTime * speed;
        float normalizedTime = Mathf.PingPong(animationTime, 1f);

        animator.Play(0, 0, normalizedTime);
        animator.Update(0f);
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
        animationTime = 0f;

        if (animator != null)
        {
            animator.Play(0, 0, 0f);
            animator.Update(0f);
        }
    }

    private void ClampSpeed()
    {
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.3f, 3f);
    }
}