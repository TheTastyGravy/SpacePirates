using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonAngles : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject leftAnglePrefab;
    public GameObject rightAnglePrefab;
    private Image leftAngleImage;
    private Image rightAngleImage;
    // Making this public makes it nessesary to change it on every button
    private float offset = 30;



    void Awake()
    {
        if (leftAngleImage == null || rightAngleImage == null)
            Init();
    }

    private void Init()
    {
        RectTransform leftAngle = Instantiate(leftAnglePrefab, transform).transform as RectTransform;
        RectTransform rightAngle = Instantiate(rightAnglePrefab, transform).transform as RectTransform;
        RectTransform rectTrans = transform as RectTransform;
        leftAngle.localPosition = new Vector2(-rectTrans.rect.width * 0.5f - offset, 0);
        rightAngle.localPosition = new Vector2(rectTrans.rect.width * 0.5f + offset, 0);

        leftAngleImage = leftAngle.GetComponentInChildren<Image>();
        rightAngleImage = rightAngle.GetComponentInChildren<Image>();
        leftAngleImage.enabled = false;
        rightAngleImage.enabled = false;
    }

    public void OnSelect(BaseEventData eventData)
	{
        if (leftAngleImage == null || rightAngleImage == null)
            Init();

        leftAngleImage.enabled = true;
        rightAngleImage.enabled = true;
    }

    public void OnDeselect(BaseEventData eventData)
	{
        leftAngleImage.enabled = false;
        rightAngleImage.enabled = false;
    }
}
