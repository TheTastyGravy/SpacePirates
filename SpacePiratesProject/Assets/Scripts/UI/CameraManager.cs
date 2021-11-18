using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public float baseShakeIntensity = 1;
    public float shakeTime = 0.5f;
    public float zoomTime = 1.5f;
    public AnimationCurve zoomCurve;
    public Camera[] cams;

    private Transform trans;
    private Vector3 startPos;
    private float currentShakeMult = 1;
    private float timePassed = 999;
    private float baseCamSize;
    private Coroutine zoomRoutine;



    void Start()
    {
        baseCamSize = cams[0].orthographicSize;
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

    public void Zoom(float zoomLevel)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);
        zoomRoutine = StartCoroutine(ZoomRoutine(baseCamSize * zoomLevel));
    }

    private IEnumerator ZoomRoutine(float zoomLevel)
    {
        float startSize = cams[0].orthographicSize;
        float t = 0;
        while (t < zoomTime)
        {
            float value = zoomCurve.Evaluate(t / zoomTime);
            foreach (Camera cam in cams)
            {
                cam.orthographicSize = Mathf.Lerp(startSize, zoomLevel, value);
            }

            t += Time.deltaTime;
            yield return null;
        }
        // Set value on end
        foreach (Camera cam in cams)
        {
            cam.orthographicSize = zoomLevel;
        }
        zoomRoutine = null;
    }
}
