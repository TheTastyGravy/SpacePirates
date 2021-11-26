using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenBar : MonoBehaviour
{
    public Image lowOxygenEffect;
    public float lowOxygenStartLevel = 0.5f;
    [Space]
    public Image bar;
    public Image arrows;
    public float arrowValue = 0.95f;
    public float fadeTime = 0.5f;
    public float arrowOffset = 20;

    public float MaxValue
	{ set => inverseMax = 1f / value; }
    private float inverseMax;
    [HideInInspector]
    public float value;
    private float lastValue = 1;
    private float timePassed = 100;
    


    void LateUpdate()
    {
        float realValue = value * inverseMax;
        bar.fillAmount = realValue;
        // Move arrows to follow the bar
        arrows.rectTransform.localPosition = new Vector2(bar.rectTransform.rect.width * -(0.5f - realValue) + arrowOffset, 0);

        // Check if we passed the arrow value
        if ((lastValue <= arrowValue && realValue > arrowValue) || (lastValue > arrowValue && realValue <= arrowValue))
		{
            timePassed = 0;
        }
        // Flip direction
        if (lastValue >= realValue || realValue <= 0)
            arrows.transform.localScale = Vector3.one;
        else
            arrows.transform.localScale = -Vector3.one;
        // Fade arrows
        if (timePassed < fadeTime)
        {
            float alpha = timePassed / fadeTime;
            arrows.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), realValue > arrowValue ? alpha : 1 - alpha);
            timePassed += Time.deltaTime;
        }
        else
        {
            arrows.color = realValue > arrowValue ? new Color(1, 1, 1, 0) : Color.white;
        }

        // Low oxygen effect alpha
        Color lowOxygenColor = lowOxygenEffect.color;
        lowOxygenColor.a = 1 - Mathf.Clamp(realValue / lowOxygenStartLevel, 0, 1);
        lowOxygenEffect.color = lowOxygenColor;

        lastValue = realValue;
    }
}
