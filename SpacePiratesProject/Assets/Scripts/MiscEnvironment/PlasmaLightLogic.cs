using UnityEngine;

public class PlasmaLightLogic : MonoBehaviour
{
    public MeshRenderer quad;
    public Gradient alpha;

    [HideInInspector] public float LightDelay;

    float elapsed;
    bool running;
    public void Initialize(float _LightDelay)
    {
        running = false;
        elapsed = 0f;
        LightDelay = _LightDelay;
        quad.material.color = new Color(1, 1, 1, 0);
    }

    private void FixedUpdate()
    {
        if (running)
        {
            elapsed += Time.fixedDeltaTime;
            quad.material.color = alpha.Evaluate(elapsed / LightDelay);
            if (elapsed >= LightDelay)
            {
                running = false;
                elapsed = 0f;
                quad.material.color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void Run()
    {
        running = true;
    }
}