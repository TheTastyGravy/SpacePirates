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

    private void FixedUpdate()
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
        Ignite.Play();
        elapsed = 0f;
    }

    public void Stop()
    {
        Ignite.Stop();
        Flame.Stop();
    }
}