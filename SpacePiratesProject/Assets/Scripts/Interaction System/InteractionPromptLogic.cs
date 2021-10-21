using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptLogic : MonoBehaviour
{
    public Image progressImage;
    public float progressLossRate = 3;
    public float popScale = 2;
    public float popTime = 0.25f;

    public Image[] baseImages;
    public Image[] selectedImages;

    [HideInInspector]
    public float interactionProgress = 0;

    private bool isBeingUsed = false;
    private bool isSelected = false;
    private float actualProgress = 0;
    private Coroutine routine;
    private bool IsBaseVisible => baseImages.Length > 0 && baseImages[0].color.a > 0;
    private bool IsSelectedVisible => selectedImages.Length > 0 && selectedImages[0].color.a > 0;



    void Awake()
    {
        progressImage.fillAmount = 0;

        foreach (var obj in selectedImages)
		{
            obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, 0);
		}
        if (!enabled)
		{
            foreach (var obj in baseImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, 0);
            }
        }
    }

    void Update()
    {
        if (routine != null)
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
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(NewFade(false, false));
    }

    private IEnumerator NewFade(bool showBase, bool showSelected)
    {
        Vector3 shrunkScale = Vector3.one - Vector3.one * popScale;
        // Dont hide what is already hidden
        bool useSelected = showSelected || IsSelectedVisible;
        bool useBase = showBase || IsBaseVisible;
        if (!useSelected && !useBase)
            yield break;

        float time = 0;
        while (time < popTime)
        {
            float val = time / popTime;
            if (useBase)
            {
                foreach (var obj in baseImages)
                {
                    obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, Mathf.Lerp(1, 0, showBase ? 1 - val : val));
                    obj.transform.localScale = Vector3.Lerp(Vector3.one, shrunkScale, showBase ? 1 - val : val);
                }
            }
            if (useSelected)
            {
                foreach (var obj in selectedImages)
                {
                    obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, Mathf.Lerp(1, 0, showSelected ? 1 - val : val));
                    obj.transform.localScale = Vector3.Lerp(Vector3.one, shrunkScale, showSelected ? 1 - val : val);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Set values to end of lerp
        if (useBase)
        {
            foreach (var obj in baseImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, showBase ? 1 : 0);
                obj.transform.localScale = showBase ? Vector3.one : shrunkScale;
            }
        }
        if (useSelected)
        {
            foreach (var obj in selectedImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, showSelected ? 1 : 0);
                obj.transform.localScale = showSelected ? Vector3.one : shrunkScale;
            }
        }

        routine = null;
        // If everything was just hiden but we are still enabled, show a prompt
        if (!showSelected && !showBase && enabled)
		{
            routine = StartCoroutine(NewFade(!isSelected, isSelected));
		}
    }

    public void InteractStart()
    {
        isBeingUsed = true;
        interactionProgress = 0;
    }

    public void InteractStop()
    {
        IEnumerator SetNextFrame()
        {
            yield return null;
            if (isActiveAndEnabled)
                isBeingUsed = false;
        }
        // Wait a frame to set isBeingUsed. This is becasue InteractionManager uses LateUpdate to determine selection
        StartCoroutine(SetNextFrame());
    }

    public void Selected()
	{
        isSelected = true;
        if (isBeingUsed && IsSelectedVisible)
            return;

        if (routine != null)
		{
            if (!enabled)
                return;
            StopCoroutine(routine);
        }
        routine = StartCoroutine(NewFade(false, true));
	}

    public void Unselected()
	{
        isSelected = false;
        if (isBeingUsed)
            return;

        if (routine != null)
        {
            if (!enabled)
                return;
            StopCoroutine(routine);
        }
        routine = StartCoroutine(NewFade(true, false));
    }

	void OnEnable()
	{
        if (routine != null)
            StopCoroutine(routine);
        // Show image based on selected
        routine = StartCoroutine(NewFade(!isSelected, isSelected));
    }

    void OnDisable()
	{
        isBeingUsed = false;
        if (gameObject.activeInHierarchy)
		{
            if (routine != null)
                StopCoroutine(routine);
            routine = StartCoroutine(NewFade(false, false));
        }
        else
		{
            foreach (var obj in baseImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, 0);
            }
            foreach (var obj in selectedImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, 0);
            }
        }
    }
}
