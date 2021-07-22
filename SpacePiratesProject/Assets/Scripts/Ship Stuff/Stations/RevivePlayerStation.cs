using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivePlayerStation : Interactable
{
	[Tooltip("How many times does a player need to interact to revive")]
	public int numberOfInteractions = 3;

	private int interactionCount;
	private PlayerHealth playerHealth;



	void Start()
	{
		playerHealth = GetComponent<PlayerHealth>();
	}
	void OnEnable()
	{
		interactionCount = 0;
	}

	protected override void OnActivate(Interactor user)
	{
		interactionCount++;

		if (interactionCount >= numberOfInteractions)
		{
			playerHealth.Revive();
		}
	}
}
