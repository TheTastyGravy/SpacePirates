using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public float baseShakeIntensity = 1;
    public float shakeTime = 0.5f;

    private Transform trans;
    private Vector3 startPos;
    private float currentShakeMult = 1;
    private float timePassed = 999;



    void Start()
    {
        trans = transform;
        startPos = trans.position;
        trans.parent = null;
    }

    void Update()
    {
        if (timePassed < shakeTime)
        {
            timePassed += Time.unscaledDeltaTime;
            if (timePassed >= shakeTime)
            {
                // At the end, reset the position
                trans.localPosition = startPos;
            }
            else
            {
                trans.position = startPos + trans.rotation * (Random.insideUnitCircle * baseShakeIntensity * currentShakeMult);
            }
        }
    }

    public void Shake(float intensityMult = 1)
    {
        currentShakeMult = intensityMult;
        timePassed = 0;
    }
}
