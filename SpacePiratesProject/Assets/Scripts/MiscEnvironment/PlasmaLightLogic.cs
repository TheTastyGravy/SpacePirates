using UnityEngine;

public class PlasmaLightLogic : MonoBehaviour
{
    public Light _Light;

    [HideInInspector] public float LightDelay;

    float elapsed;
    bool running;
    public void Initialize(float _LightDelay)
    {
        running = false;
        elapsed = 0f;
        LightDelay = _LightDelay;
        _Light.enabled = false;
    }

    private void FixedUpdate()
    {
        if (running)
        {
            elapsed += Time.fixedDeltaTime;
            if (elapsed >= LightDelay)
            {
                elapsed = 0f;
                _Light.enabled = false;
            }
        }
    }

    public void Run()
    {
        running = true;
        _Light.enabled = true;
    }
}