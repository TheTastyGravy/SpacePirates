using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSwitch : Interactable
{
    public BasicDelegate OnActivated;
    [Tooltip("How long the switch is disabled for after being used")]
    public float interactionCooldown = 1;

	[HideInInspector]
	public bool forceDisabled = false;



	private void Reenable()
	{
		if (forceDisabled)
			return;

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
