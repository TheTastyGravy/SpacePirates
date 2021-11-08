using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FuelDeposit : Interactable
{
    public float timeToDeposit = 1;
    public EventReference refuelEvent;

    private float timePassed = 0;
    private FMOD.Studio.EventInstance refuelEventInstance;

    public BasicDelegate OnFuelDeposited;



	void Start()
	{
        refuelEventInstance = RuntimeManager.CreateInstance(refuelEvent);
        refuelEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
    }

    protected override void OnDestroy()
	{
        base.OnDestroy();
        refuelEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        refuelEventInstance.release();
    }

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
        // Stop sound effect
        refuelEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

	protected override void OnInteractionStart()
	{
        timePassed = 0;
        // Start sound effect
        refuelEventInstance.start();
    }

	protected override void OnButtonUp()
	{
        currentInteractor.EndInteraction();
        // Stop sound effect
        refuelEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        // Players can only interact if they are holding fuel
        return interactor.HeldGrabbable != null && interactor.HeldGrabbable.CompareTag("Fuel");
    }
}
