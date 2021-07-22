using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedbayStation : Interactable
{
	public float healthPerUse = 25;
	public float damagedHealthPerUse = 10;



	protected override void OnActivate(Interactor user)
	{
		PlayerHealth playerHealth = user.GetComponent<PlayerHealth>();

		if (playerHealth.Health == playerHealth.maxHealth)
		{
			// Player is at full health, do nothing
		}
		else
		{
			// Heal player
			playerHealth.UpdateHealth(healthPerUse);
		}
	}
}
