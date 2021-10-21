using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelCell : MonoBehaviour
{
    public Image outer;
    public Image filled;
    public float speed = 10;

    private Vector3 targetScale;
    private bool done = false;



    internal void SetTarget(Vector3 target)
	{
        targetScale = target;
        done = false;
    }

    internal void SetColor(Color color)
	{
        outer.color = color;
        filled.color = color;
    }

    void Start()
    {
        outer.enabled = true;
        filled.enabled = true;
        filled.transform.localScale = Vector3.zero;
        targetScale = Vector3.zero;
    }

    void Update()
    {
        if (done)
            return;

        filled.transform.localScale = Vector3.Lerp(filled.transform.localScale, targetScale, Time.deltaTime * speed);

        if (Vector3.SqrMagnitude(filled.transform.localScale - targetScale) < 0.05f)
		{
            filled.transform.localScale = targetScale;
            done = true;
        }
    }
}
