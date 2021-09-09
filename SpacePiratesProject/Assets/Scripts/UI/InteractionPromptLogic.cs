using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionPromptLogic : MonoBehaviour
{
    public Image promptImage;
    public Image progressImage;
    public float progressLossRate = 3;
    public float popScale = 2;
    public float popTime = 0.25f;
    public TextMeshProUGUI _text;

    [HideInInspector]
    public float interactionProgress = 0;


    private bool isBeingUsed = false;
    private bool isPoping = false;
    private float actualProgress = 0;



    void Awake()
    {
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
        _text.enabled = false;

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
        // Make fully transparent and wait a moment before resetting
        promptImage.color = trans;
        progressImage.color = trans;
        yield return new WaitForSeconds(0.2f);

        // Reset images
        promptImage.color = Color.white;
        promptImage.transform.localScale = Vector3.one;
        progressImage.color = Color.white;
        progressImage.transform.localScale = Vector3.one;
        // Reset progress
        progressImage.fillAmount = 0;
        actualProgress = 0;
        // If we have been disabled, hide the images
        if (!enabled)
		{
            promptImage.enabled = false;
            progressImage.enabled = false;
        }
		else
		{
            _text.enabled = true;
        }

        isPoping = false;
        isBeingUsed = false;
    }


    public void InteractStart()
    {
        isBeingUsed = true;
        _text.enabled = false;
        interactionProgress = 0;
    }

    public void InteractStop()
    {
        isBeingUsed = false;

        if (!isPoping && enabled)
		{
            _text.enabled = true;
        }
    }

    public void SelectStart()
	{
    }

    public void SelectStop()
	{
        if (isPoping)
            return;

        isBeingUsed = false;
    }


	void OnEnable()
	{
        promptImage.enabled = true;
        progressImage.enabled = true;
        _text.enabled = true;
    }

    void OnDisable()
	{
        if (!isPoping)
		{
            promptImage.enabled = false;
            progressImage.enabled = false;
        }
        
        _text.enabled = false;
    }
}
