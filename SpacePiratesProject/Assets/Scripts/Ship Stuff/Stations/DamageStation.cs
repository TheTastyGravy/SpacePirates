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



	void Start()
	{
		// This should only be usable after taking damage
		SetIsUsable(false);
	}

	public void Damage()
	{
		damageLevel++;
		if (damageLevel > maxDamageLevel)
			damageLevel = maxDamageLevel;

		SetIsUsable(true);
	}

	protected override void OnActivate(Interactor user)
	{
		currentRepairCount++;
		if (currentRepairCount >= repairCount)
		{
			currentRepairCount = 0;
			damageLevel--;

			if (damageLevel <= 0)
			{
				SetIsUsable(false);
			}
		}
	}
}
