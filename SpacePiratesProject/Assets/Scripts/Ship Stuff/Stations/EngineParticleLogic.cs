using UnityEngine;

public class EngineParticleLogic : MonoBehaviour
{
    public ParticleSystem Ignite, Flame;
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

        Ignite.Play();
        elapsed = 0f;
    }

    public void Stop()
    {
        Ignite.Stop();
        Flame.Stop();
    }
}