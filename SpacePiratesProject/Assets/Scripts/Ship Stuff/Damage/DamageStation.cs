using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class DamageStation : Interactable
{
	[Space]
	public ParticleSystem[] effects;
	public EventReference repairEvent;
	public EventReference damageEvent;
	[Space]
	public int maxDamageLevel = 1;
	public float timeToRepair = 1;

    private int damageLevel = 0;
	public int DamageLevel { get => damageLevel; }

	private float currentRepairTime = 0;
	private FMOD.Studio.EventInstance repairEventInstance;

	public BasicDelegate OnDamageTaken;
	public BasicDelegate OnDamageRepaired;



	void Start()
	{
		// This should only be usable after taking damage
		enabled = false;

		repairEventInstance = RuntimeManager.CreateInstance(repairEvent);
		repairEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		repairEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		repairEventInstance.release();
	}

	void Update()
	{
		// If we are being interacted with
		if (IsBeingUsed)
		{
			currentRepairTime += Time.deltaTime;
			if (currentRepairTime >= timeToRepair)
			{
				damageLevel--;
				OnDamageRepaired?.Invoke();

				// Unlock the player and update the interactor
				interactionPrompt.Pop();
				currentInteractor.EndInteraction();

				if (damageLevel <= 0)
				{
					enabled = false;
				}

				foreach (var obj in effects)
				{
					obj.Stop();
				}
				// Stop sound effect
				repairEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}

			// Update interaction prompt progress
			interactionPrompt.interactionProgress = currentRepairTime / timeToRepair;
		}
	}

	public void Damage()
	{
		if (damageLevel >= maxDamageLevel)
			return;

		damageLevel++;
		enabled = true;
		OnDamageTaken?.Invoke();

		foreach (var obj in effects)
		{
			obj.Play();
		}
		// Play sound effect
		RuntimeManager.PlayOneShot(damageEvent);
	}

	protected override void OnInteractionStart()
	{
		currentRepairTime = 0;
		// Start sound effect
		repairEventInstance.start();
	}

	protected override void OnButtonUp()
	{
		currentInteractor.EndInteraction();
		// Stop sound effect
		repairEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
	{
		button = Player.Control.A_PRESSED;
		return true;
	}
}
