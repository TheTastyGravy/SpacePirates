using UnityEngine;

public class EngineParticleLogic : MonoBehaviour
{
    public ParticleSystem Ignite, Flame, Smoke;
    [Space]
    public float Delay;

    float elapsed;

	private void OnEnable()
    {
        elapsed = 0f;
    }

    private void LateUpdate()
    {
        if (elapsed < Delay)
        {
            elapsed += Time.fixedDeltaTime;
            if (elapsed > Delay)
                Flame.Play();
        }
    }

    public void Run()
    {
        // Ignore if we are already running
        if (Ignite.isPlaying || Flame.isPlaying)
            return;

        Smoke.Stop();
        Ignite.Play();
        elapsed = 0f;
    }

    public void Stop()
    {
        if (Smoke.isPlaying)
            return;

        Ignite.Stop();
        Flame.Stop();
        Smoke.Play();
    }
}