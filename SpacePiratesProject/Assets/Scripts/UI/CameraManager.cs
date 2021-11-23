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
    private Vector3 basePos;
    private float currentShakeMult = 1;
    private float timePassed = 999;
    private float baseCamSize;
    private Coroutine zoomRoutine;



    void Start()
    {
        baseCamSize = cams[0].orthographicSize;
        trans = transform;
        basePos = trans.position;
        trans.parent = null;
    }

    void Update()
    {
        timePassed += Time.unscaledDeltaTime;
        if (timePassed >= shakeTime)
        {
            // At the end, reset the position
            trans.localPosition = basePos;
        }
        else
        {
            trans.position = basePos + trans.rotation * (Random.insideUnitCircle * baseShakeIntensity * currentShakeMult);
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
        Vector3 initPos = basePos;
        Vector3 up = cams[0].transform.up;
        Vector3 right = cams[0].transform.right;
        float t = 0;
        while (t < zoomTime)
        {
            // Move the base position to pin the top left corner in place
            float newSize = Mathf.Lerp(startSize, zoomLevel, zoomCurve.Evaluate(t / zoomTime));
            float diff = startSize - newSize;
            basePos = initPos + up * diff + right * diff * cams[0].aspect;

            foreach (Camera cam in cams)
            {
                cam.orthographicSize = newSize;
                cam.transform.position = basePos;
            }
            t += Time.deltaTime;
            yield return null;
        }
        // Set value on end
        basePos = initPos + up * (startSize - zoomLevel) + right * (startSize - zoomLevel) * cams[0].aspect;
        foreach (Camera cam in cams)
        {
            cam.orthographicSize = zoomLevel;
        }
        zoomRoutine = null;
    }
}
