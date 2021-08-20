using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStation : Interactable
{
	public int maxDamageLevel = 1;
	public int repairCount = 2;

    private int damageLevel = 0;
	public int DamageLevel { get => damageLevel; }

	private int currentRepairCount = 0;
	private ParticleSystem effect;



	void Start()
	{
		// This should only be usable after taking damage
		enabled = false;

		effect = GetComponentInChildren<ParticleSystem>();
	}

	public void Damage()
	{
		damageLevel++;
		if (damageLevel > maxDamageLevel)
			damageLevel = maxDamageLevel;

		enabled = true;

		if (effect != null)
		{
			effect.Play();
		}

		SoundManager.Instance.Play("StationDamage", false);
	}

	protected override void OnInteractStart(Interactor interactor)
	{
		currentRepairCount++;
		if (currentRepairCount >= repairCount)
		{
			currentRepairCount = 0;
			damageLevel--;

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
	}

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
	{
		button = Player.Control.A_PRESSED;
		return true;
	}
}
