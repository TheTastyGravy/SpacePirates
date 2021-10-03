using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelDeposit : Interactable
{
    public float timeToDeposit = 1;

    private float timePassed = 0;

    public BasicDelegate OnFuelDeposited;



    void Update()
    {
        // If we are being interacted with
        if (IsBeingUsed)
		{
            timePassed += Time.deltaTime;
            if (timePassed >= timeToDeposit)
			{
                OnDepositComplete();
			}

            interactionPrompt.interactionProgress = timePassed / timeToDeposit;
		}
    }

    private void OnDepositComplete()
	{
        interactionPrompt.Pop();

        // Destroy the held object
        Grabbable held = currentInteractor.HeldGrabbable;
        currentInteractor.Drop();
        Destroy(held.gameObject);

        currentInteractor.EndInteraction();

        OnFuelDeposited?.Invoke();
    }

	protected override void OnInteractionStart()
	{
        timePassed = 0;
    }

	protected override void OnButtonUp()
	{
        currentInteractor.EndInteraction();
	}

    protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        // Players can only interact if they are holding fuel
        return interactor.HeldGrabbable != null && interactor.HeldGrabbable.CompareTag("Fuel");
    }
}
