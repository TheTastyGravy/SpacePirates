using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReactorFuelGen : Interactable
{
    public GameObject fuelPrefab;
    [Tooltip("How long it takes to generate a fuel")]
    public float timeToGenerate = 4;
    public TextMeshProUGUI timerText;

    // Are we currently generating fuel?
    private bool isActive = true;

    private bool hasFuel = false;
    private float fuelTimePassed = 0;



	void Start()
	{
        interactionPrompt.enabled = false;
        if (timerText != null)
		{
            timerText.text = (timeToGenerate + 1).ToString();
        }
	}

	void Update()
    {
        if (isActive && !hasFuel)
		{
            fuelTimePassed += Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = (timeToGenerate - fuelTimePassed + 1).ToString("0");
            }

            if (fuelTimePassed >= timeToGenerate)
			{
                fuelTimePassed = 0;
                hasFuel = true;
                interactionPrompt.enabled = true;
			}
        }
    }

    internal void SetActive(bool value)
	{
        if (isActive != value)
		{
            isActive = value;
            if (!hasFuel && interactionPrompt != null)
                interactionPrompt.Pop(isActive);
        }
	}

	protected override void OnInteractionStart()
	{
        interactionPrompt.Pop();
        interactionPrompt.enabled = false;

        Grabbable fuelGrabbable = Instantiate(fuelPrefab).GetComponent<Grabbable>();
        currentInteractor.Pickup(fuelGrabbable);
        
        hasFuel = false;

        currentInteractor.EndInteraction();
        if (!hasFuel && interactionPrompt != null)
            interactionPrompt.Pop(isActive);
    }

    protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
	{
        button = Player.Control.A_PRESSED;
        return hasFuel && interactor.HeldGrabbable == null;
    }
}
