using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretActivate : Interactable
{
    public delegate void InteractorDelegate(Interactor interactor);
    public InteractorDelegate OnInteract;



	protected override void OnInteractionStart()
	{
        interactionPrompt.Pop();
        OnInteract?.Invoke(currentInteractor);
	}

    protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
