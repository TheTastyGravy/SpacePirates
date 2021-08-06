using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedbayStation : Interactable
{
	public float healthPerUse = 25;
	public float damagedHealthPerUse = 10;

	private DamageStation damage;



	void Awake()
	{
		damage = transform.parent.GetComponentInChildren<DamageStation>();
	}

	protected override void OnInteract(Interactor user)
	{
		Character playerHealth = user.GetComponent<Character>();

		if (playerHealth.Health == playerHealth.HealthMax)
		{
			// Player is at full health, do nothing
		}
		else
		{
			bool isDamaged = damage != null;
			if (isDamaged)
				isDamaged = damage.DamageLevel > 0;

			// Heal player
			playerHealth.ApplyHealthModifier(isDamaged ? damagedHealthPerUse : healthPerUse);
			// Increase power usage for this tick
			PowerManager.Instance.tempEnergyUsage++;

			SoundManager.Instance.Play("Medbay", false);
		}
	}
}
