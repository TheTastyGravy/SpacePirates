﻿using UnityEngine;

public class RunLightning : MonoBehaviour
{
    public PlasmaLightLogic[] _Lights;
    [Space]
    [SerializeField] LightArrays[] LightTriggers;
    public float LightDelay;
    [Space]
    public float RandomBufferMin;
    public float RandomBufferMax;

    float elapsed, targetTime;
    int iterative, selective;
    private void OnEnable()
    {
        elapsed = 0f;
        iterative = 0;
        selective = Random.Range(0, LightTriggers.Length);
        targetTime = GetStartTime();
        InitializeLights();
    }
    float GetStartTime() => LightTriggers[selective].LightTriggers[0] + Random.Range(RandomBufferMin, RandomBufferMax);
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
        if (iterative >= LightTriggers[selective].LightTriggers.Length)
            iterative = 0;
        if (iterative == 0)
        {
            selective = Random.Range(0, LightTriggers.Length);
            targetTime = GetStartTime();
        }
        else
            targetTime = LightTriggers[selective].LightTriggers[iterative];
    }
    void RunLight() => _Lights[Random.Range(0, _Lights.Length)].Run();
}
[System.Serializable]
struct LightArrays
{
    public string Heading;
    public float[] LightTriggers;
}