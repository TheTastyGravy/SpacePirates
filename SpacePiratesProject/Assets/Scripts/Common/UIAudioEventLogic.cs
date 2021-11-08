using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;

public class UIAudioEventLogic : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    public EventReference highlightEvent;
    public EventReference selectEvent;
	public bool useSlider = false;

	public static bool IgnoreNextHighlight = false;



	void Awake()
	{
		if (useSlider)
		{
			GetComponent<Slider>().onValueChanged.AddListener(delegate { OnValueChanged(); });
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (IgnoreNextHighlight)
		{
			IgnoreNextHighlight = false;
			return;
		}

		RuntimeManager.PlayOneShot(highlightEvent);
	}

	public void OnSubmit(BaseEventData eventData)
	{
		if (!useSlider)
			RuntimeManager.PlayOneShot(selectEvent);
	}

	private void OnValueChanged()
	{
		RuntimeManager.PlayOneShot(selectEvent);
	}
}
