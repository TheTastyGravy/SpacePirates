using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorSwitch : Interactable
{
    internal BasicDelegate OnActivated;

    [Tooltip("How long the switch is disabled for after being used")]
    public float interactionCooldown = 1;

    private float timePassed;



	void Start()
	{
		timePassed = interactionCooldown;
	}

	void Update()
	{
		if (timePassed < interactionCooldown)
		{
			timePassed += Time.deltaTime;

			if (timePassed >= interactionCooldown)
			{
				ReregisterInteractions();
			}
		}
	}


	protected override void OnInteractStart(Interactor interactor)
	{
        OnActivated?.Invoke();
		timePassed = 0;
        ReregisterInteractions();
	}

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return timePassed >= interactionCooldown;
    }
}
