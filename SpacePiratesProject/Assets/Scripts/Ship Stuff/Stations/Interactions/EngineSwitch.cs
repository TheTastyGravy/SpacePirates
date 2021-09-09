using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSwitch : Interactable
{
    internal EngineStation engine;

    // Invoked when we have been turned on or off
    internal BasicDelegate OnActivated;



	protected override void OnInteractStart(Interactor interactor)
	{
        if (engine.IsTurnedOn)
		{
            // Turn the engine off
            OnActivated?.Invoke();
            interactionPrompt.Pop();
        }
		else
		{
            //start minigame to turn on
            OnActivated?.Invoke();
            interactionPrompt.Pop();
        }
	}

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
