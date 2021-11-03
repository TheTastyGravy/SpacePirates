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
    public Image[] disabledImages;

    [HideInInspector]
    public float interactionProgress = 0;
    private Interactable m_interactable;
    public Interactable Interactable { get => m_interactable; internal set { m_interactable = value; } }
    public bool IsBeingUsed => isBeingUsed;
    public bool IsSelected => isSelected;

    private bool isBeingUsed = false;
    private bool isSelected = false;
    private float actualProgress = 0;
    private Coroutine routine;
    private bool IsBaseVisible => baseImages.Length > 0 && baseImages[0].color.a > 0;
    private bool IsSelectedVisible => selectedImages.Length > 0 && selectedImages[0].color.a > 0;
    private bool IsDisabledVisible => disabledImages.Length > 0 && disabledImages[0].color.a > 0;

    public BasicDelegate OnSelected;
    public BasicDelegate OnUnselected;



    void Awake()
    {
        selectedImages[0].fillAmount = 1;
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
		else
		{
            foreach (var obj in disabledImages)
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

        selectedImages[0].fillAmount = 1 - actualProgress;
        progressImage.fillAmount = actualProgress;
    }

    public void Pop(bool showDisabled = true)
	{
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(NewFade(false, false, showDisabled));
    }

    public void PopV2()
	{
        if (!enabled)
            return;

        if (routine != null)
        {
            StopCoroutine(routine);
        }

        bool useSelected = IsSelectedVisible;
        bool useBase = IsBaseVisible;
        bool useDisabled = IsDisabledVisible;

        if (useSelected)
		{
            foreach (var obj in selectedImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, 0);
            }
        }
        if (useBase)
		{
            foreach (var obj in baseImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, 0);
            }
        }
        if (useDisabled)
		{
            foreach (var obj in disabledImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, 0);
            }
        }

        routine = StartCoroutine(NewFade(useBase, useSelected, useDisabled));
    }

    private IEnumerator NewFade(bool showBase, bool showSelected, bool showDisabled = false)
    {
        Vector3 shrunkScale = Vector3.one - Vector3.one * popScale;
        // Dont hide what is already hidden
        bool useSelected = showSelected || IsSelectedVisible;
        bool useBase = showBase || IsBaseVisible;
        bool useDisabled = showDisabled || IsDisabledVisible;
        if (!useSelected && !useBase && !useDisabled)
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
            if (useDisabled)
            {
                foreach (var obj in disabledImages)
                {
                    obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, Mathf.Lerp(1, 0, showDisabled ? 1 - val : val));
                    obj.transform.localScale = Vector3.Lerp(Vector3.one, shrunkScale, showDisabled ? 1 - val : val);
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
        if (useDisabled)
        {
            foreach (var obj in disabledImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, showDisabled ? 1 : 0);
                obj.transform.localScale = showDisabled ? Vector3.one : shrunkScale;
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
        if (OnSelected != null)
            OnSelected.Invoke();

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
        if (OnUnselected != null)
            OnUnselected.Invoke();

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
            routine = StartCoroutine(NewFade(false, false, true));
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
            foreach (var obj in disabledImages)
            {
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, 1);
                obj.transform.localScale = Vector3.one;
            }
        }
    }
}
