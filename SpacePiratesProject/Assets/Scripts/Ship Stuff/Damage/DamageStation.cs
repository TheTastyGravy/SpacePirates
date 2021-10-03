using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStation : Interactable
{
	public int maxDamageLevel = 1;
	public float timeToRepair = 1;

    private int damageLevel = 0;
	public int DamageLevel { get => damageLevel; }

	private float currentRepairTime = 0;
	private ParticleSystem effect;

	public BasicDelegate OnDamageTaken;
	public BasicDelegate OnDamageRepaired;



	void Start()
	{
		// This should only be usable after taking damage
		enabled = false;

		effect = GetComponentInChildren<ParticleSystem>();
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

				if (effect != null)
				{
					effect.Stop();
				}

				SoundManager.Instance.Play("Repair", false);
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

		if (effect != null)
		{
			effect.Play();
		}

		SoundManager.Instance.Play("StationDamage", false);
	}

	protected override void OnInteractionStart()
	{
		currentRepairTime = 0;
	}

	protected override void OnButtonUp()
	{
		currentInteractor.EndInteraction();
	}

	protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
	{
		button = Player.Control.A_PRESSED;
		return true;
	}
}
