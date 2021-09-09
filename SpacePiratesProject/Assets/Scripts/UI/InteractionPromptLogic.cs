using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptLogic : MonoBehaviour
{
    public Image promptImage;
    public Image progressImage;
    public float progressLossRate = 3;
    public float popScale = 2;
    public float popTime = 0.25f;

    [HideInInspector]
    public float interactionProgress = 0;


    private bool isBeingUsed = false;
    private bool isPoping = false;
    private float actualProgress = 0;



    void Awake()
    {
        promptImage.enabled = false;
        progressImage.fillAmount = 0;
    }

    void Update()
    {
        if (isPoping)
            return;

        if (isBeingUsed)
		{
            actualProgress = interactionProgress;
		}
		else
		{
            actualProgress -= progressLossRate * Time.deltaTime;
		}

        progressImage.fillAmount = actualProgress;
    }

    public void Pop()
	{
        StartCoroutine(ExpandAndFade());
    }

    private IEnumerator ExpandAndFade()
	{
        isPoping = true;

        Color trans = new Color(1, 1, 1, 0);

        float timePassed = 0;
        while (timePassed < popTime)
		{
            // Lerp transparency and scale
            float val = timePassed / popTime;
            promptImage.color = Color.Lerp(Color.white, trans, val);
            promptImage.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * popScale, val);
            progressImage.color = Color.Lerp(Color.white, trans, val);
            progressImage.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * popScale, val);

            timePassed += Time.deltaTime;
            yield return null;
		}

        //reset images
        promptImage.color = Color.white;
        promptImage.transform.localScale = Vector3.one;
        promptImage.enabled = false;
        progressImage.color = Color.white;
        progressImage.transform.localScale = Vector3.one;
        progressImage.enabled = false;

        isPoping = false;
    }


    public void InteractStart()
    {
        isBeingUsed = true;
        interactionProgress = 0;
    }

    public void InteractStop()
    {
        isBeingUsed = false;
    }

    public void SelectStart()
	{
        promptImage.enabled = true;
        progressImage.enabled = true;
        progressImage.fillAmount = 0;
        actualProgress = 0;
    }

    public void SelectStop()
	{
        if (isPoping)
            return;

        promptImage.enabled = false;
        progressImage.enabled = false;
        isBeingUsed = false;
    }
}
