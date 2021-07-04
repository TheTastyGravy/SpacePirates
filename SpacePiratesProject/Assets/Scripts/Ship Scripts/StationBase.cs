using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StationBase : IInteractable
{
    // The root transform for the ship this station belongs to
    public Transform shipExternal;

    protected bool isActive = false;
    protected Interactor currentUser = null;

    private BasicController currentPlayerController = null;


	public sealed override void Activate(Interactor user)
	{
        isActive = !isActive;
        currentUser = isActive ? user : null;

        if (isActive)
		{
            //get and disable player controller
            currentPlayerController = user.GetComponent<BasicController>();
            currentPlayerController.canMove = false;

            OnActivated();
		}
		else
		{
            //enable the player controller
            currentPlayerController.canMove = true;
            currentPlayerController = null;

            OnDeactivated();
		}
    }

    protected abstract void OnActivated();
    protected abstract void OnDeactivated();
}
