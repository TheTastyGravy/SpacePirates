using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSwitch : Interactable
{
    public BasicDelegate OnActivated;

    [Tooltip("How long the switch is disabled for after being used")]
    public float interactionCooldown = 1;



	private void Reenable()
	{
		if (enabled)
			interactionPrompt.enabled = true;

		enabled = true;
	}

	protected override void OnInteractionStart()
	{
        OnActivated?.Invoke();
		
		// Pop interaction prompt, and start cooldown
		interactionPrompt.Pop();
		
		currentInteractor.EndInteraction();

		enabled = false;
		Invoke(nameof(Reenable), interactionCooldown);
	}

	protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
	{
        button = Player.Control.A_PRESSED;
        return true;
    }
}
