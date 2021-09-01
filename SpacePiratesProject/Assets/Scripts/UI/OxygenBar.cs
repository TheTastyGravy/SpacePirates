using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBar : MonoBehaviour
{
    public RectTransform bar;

    public float MaxValue
	{ set => inverseMax = 1f / value; }
    private float inverseMax;
    [HideInInspector]
    public float value;



    void LateUpdate()
    {
        bar.offsetMax = new Vector2(-(1.0f - value * inverseMax) * (bar.parent as RectTransform).rect.width, bar.offsetMax.y);
    }
}
