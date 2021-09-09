using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorFuelGen : Interactable
{
    public GameObject fuelPrefab;
    [Tooltip("How long it takes to generate a fuel")]
    public float timeToGenerate = 4;

    // Are we currently generating fuel?
    internal bool isActive = true;

    private bool hasFuel = false;
    private float fuelTimePassed = 0;



	void Start()
	{
        interactionPrompt.enabled = false;
	}

	void Update()
    {
        if (isActive && !hasFuel)
		{
            fuelTimePassed += Time.deltaTime;

            if (fuelTimePassed >= timeToGenerate)
			{
                fuelTimePassed = 0;
                hasFuel = true;
                interactionPrompt.enabled = true;
                ReregisterInteractions();
			}
		}
    }


	protected override void OnInteractStart(Interactor interactor)
	{
        interactionPrompt.Pop();
        interactionPrompt.enabled = false;

        Grabbable fuelGrabbable = Instantiate(fuelPrefab).GetComponent<Grabbable>();
        interactor.Pickup(fuelGrabbable);
        
        hasFuel = false;
        ReregisterInteractions();
	}

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return hasFuel && interactor.HeldGrabbable == null;
    }
}
