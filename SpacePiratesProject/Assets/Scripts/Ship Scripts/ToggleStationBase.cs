using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleStationBase : IInteractable
{
    protected bool isActive = false;
    protected Interactor currentUser = null;

    private BasicController currentPlayerController = null;


    public sealed override void OnActivateDown(Interactor user)
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
    public sealed override void OnActivateUp(Interactor user)
    {
    }


    protected virtual void OnActivated() { }
    protected virtual void OnDeactivated() { }
}
