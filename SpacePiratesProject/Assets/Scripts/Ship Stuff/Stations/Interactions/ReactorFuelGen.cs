using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReactorFuelGen : Interactable
{
    public GameObject fuelPrefab;
    [Tooltip("How long it takes to generate a fuel")]
    public float timeToGenerate = 4;
    public Image progressImageBase;
    public Image progressImageFill;
    public bool startWithFuel = true;

    // Are we currently generating fuel?
    private bool isActive = true;

    private bool hasFuel = false;
    private float fuelTimePassed = 0;



	void Start()
	{
        hasFuel = startWithFuel;
        interactionPrompt.enabled = hasFuel;
	}

	void Update()
    {
        if (isActive && !hasFuel)
		{
            fuelTimePassed += Time.deltaTime;

            float value = fuelTimePassed / timeToGenerate;
            progressImageBase.fillAmount = 1 - value;
            progressImageFill.fillAmount = value;

            if (fuelTimePassed >= timeToGenerate)
			{
                // Fuel is ready
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

        // Make player pickup a new fuel grabbable
        Grabbable fuelGrabbable = Instantiate(fuelPrefab).GetComponent<Grabbable>();
        currentInteractor.Pickup(fuelGrabbable);
        
        hasFuel = false;

        currentInteractor.EndInteraction();
        // Fix interaction prompt
        if (!hasFuel && interactionPrompt != null)
            interactionPrompt.Pop(isActive);
    }

    protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
	{
        button = Player.Control.A_PRESSED;
        return hasFuel && interactor.HeldGrabbable == null;
    }
}
