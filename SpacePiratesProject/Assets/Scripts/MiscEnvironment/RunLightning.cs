using UnityEngine;

public class RunLightning : MonoBehaviour
{
    public PlasmaLightLogic[] _Lights;
    [Space]
    public float[] LightTriggers;
    public float LightDelay;
    [Space]
    public float RandomBufferMin;
    public float RandomBufferMax;

    float elapsed, targetTime;
    int iterative;
    private void OnEnable()
    {
        elapsed = 0f;
        iterative = 0;
        targetTime = GetStartTime();
        InitializeLights();
    }
    float GetStartTime() => LightTriggers[0] + Random.Range(RandomBufferMin, RandomBufferMax);
    void InitializeLights()
    {
        for (int i = 0; i < _Lights.Length; i++)
            _Lights[i].Initialize(LightDelay);
    }

    private void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;
        if(elapsed >= targetTime)
        {
            UpdateTimer();
            RunLight();
        }
    }
    void UpdateTimer()
    {
        elapsed = 0f;
        iterative++;
        if (iterative >= LightTriggers.Length)
            iterative = 0;
        if(iterative == 0)
            targetTime = GetStartTime();
        else
            targetTime = LightTriggers[iterative];
    }
    void RunLight() => _Lights[Random.Range(0, _Lights.Length)].Run();
}
