using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptLogic : MonoBehaviour
{
    public GameObject promptRoot;
    public Image progressImage;
    public float progressLossRate = 3;
    public float popScale = 2;
    public float popTime = 0.25f;

    [HideInInspector]
    public float interactionProgress = 0;

    private Image[] promptImages;

    private bool isBeingUsed = false;
    private bool isPoping = false;
    private float actualProgress = 0;



    void Awake()
    {
        promptImages = promptRoot.GetComponentsInChildren<Image>();
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
            foreach (var obj in promptImages)
			{
                obj.color = Color.Lerp(Color.white, trans, val);
                obj.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * popScale, val);
            }
            progressImage.color = Color.Lerp(Color.white, trans, val);
            progressImage.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * popScale, val);

            timePassed += Time.deltaTime;
            yield return null;
		}
        // Make fully transparent and wait a moment before resetting
        foreach (var obj in promptImages)
        {
            obj.color = trans;
        }
        progressImage.color = trans;
        yield return new WaitForSeconds(0.2f);

        // Reset images
        foreach (var obj in promptImages)
        {
            obj.color = Color.white;
            obj.transform.localScale = Vector3.one;
        }
        progressImage.color = Color.white;
        progressImage.transform.localScale = Vector3.one;
        // Reset progress
        progressImage.fillAmount = 0;
        actualProgress = 0;
        // If we have been disabled, hide the images
        if (!enabled)
		{
            foreach (var obj in promptImages)
            {
                obj.enabled = false;
            }
            progressImage.enabled = false;
        }

        isPoping = false;
        isBeingUsed = false;
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

	void OnEnable()
	{
        foreach (var obj in promptImages)
        {
            obj.enabled = true;
        }
        progressImage.enabled = true;
    }

    void OnDisable()
	{
        if (!isPoping)
		{
            foreach (var obj in promptImages)
            {
                obj.enabled = false;
            }
            progressImage.enabled = false;
        }
    }
}
