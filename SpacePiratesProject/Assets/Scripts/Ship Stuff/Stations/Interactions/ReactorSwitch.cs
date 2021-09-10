using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorSwitch : Interactable
{
    internal BasicDelegate OnActivated;

    [Tooltip("How long the switch is disabled for after being used")]
    public float interactionCooldown = 1;

	private bool canBeUsed = true;



	private void Reenable()
	{
		if (enabled)
			interactionPrompt.enabled = true;

		canBeUsed = true;
		ReregisterInteractions();
	}

	protected override void OnInteractStart(Interactor interactor)
	{
        OnActivated?.Invoke();
		canBeUsed = false;
		ReregisterInteractions();

		// Pop interaction prompt, and start cooldown
		interactionPrompt.Pop();
		interactionPrompt.enabled = false;
		Invoke(nameof(Reenable), interactionCooldown);
	}

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return canBeUsed;
    }
}
