using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretActivate : Interactable
{
    public delegate void InteractorDelegate(Interactor interactor);
    public InteractorDelegate OnInteract;



	protected override void OnInteractStart(Interactor interactor)
	{
        interactionPrompt.Pop();
        OnInteract?.Invoke(interactor);
	}

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
