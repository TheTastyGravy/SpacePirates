using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelDeposit : Interactable
{
    public float timeToDeposit = 1;

    // If not null, we are being interacted with
    private Interactor currentInteractor = null;
    private float timePassed = 0;

    public BasicDelegate OnFuelDeposited;



    void Update()
    {
        // If we are being interacted with
        if (currentInteractor != null)
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

        // Unlock the player and update the interactor
        currentInteractor.Player.Character.enabled = true;
        currentInteractor.UpdateRegistry();
        currentInteractor = null;

        OnFuelDeposited?.Invoke();
    }


	protected override void OnInteractStart(Interactor interactor)
	{
        currentInteractor = interactor;
        timePassed = 0;
        // Lock the player
        currentInteractor.Player.Character.enabled = false;
    }
	protected override void OnInteractStop(Interactor interactor)
	{
        // Unlock player
        currentInteractor.Player.Character.enabled = true;
        currentInteractor = null;
    }

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        // Players can only interact if they are holding fuel
        return interactor.HeldGrabbable != null && interactor.HeldGrabbable.CompareTag("Fuel");
    }
}
