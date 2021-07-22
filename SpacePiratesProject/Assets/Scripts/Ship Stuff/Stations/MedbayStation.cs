using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedbayStation : Interactable
{
	public float healthPerUse = 25;
	public float damagedHealthPerUse = 10;

	public DamageStation damage;



	protected override void OnActivate(Interactor user)
	{
		PlayerHealth playerHealth = user.GetComponent<PlayerHealth>();

		if (playerHealth.Health == playerHealth.maxHealth)
		{
			// Player is at full health, do nothing
		}
		else
		{
			bool isDamaged = damage != null;
			if (!isDamaged)
				isDamaged = damage.DamageLevel > 0;

			// Heal player
			playerHealth.UpdateHealth(isDamaged ? damagedHealthPerUse : healthPerUse);
		}
	}
}
