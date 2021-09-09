using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorSwitch : Interactable
{
    internal BasicDelegate OnActivated;

    [Tooltip("How long the switch is disabled for after being used")]
    public float interactionCooldown = 1;




	void Start()
	{
		
	}

	

	private void Reenable()
	{
		enabled = true;
	}


	protected override void OnInteractStart(Interactor interactor)
	{
        OnActivated?.Invoke();

		interactionPrompt.Pop();
		enabled = false;
		//ReregisterInteractions();

		Invoke(nameof(Reenable), interactionCooldown);
	}

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
