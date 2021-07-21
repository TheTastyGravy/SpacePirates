using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HoldStationBase : IInteractable
{
    protected bool isActive = false;
    protected Interactor currentUser = null;

    private BasicController currentPlayerController = null;



    public sealed override void OnActivateDown(Interactor user)
    {
        isActive = true;
        currentUser = user;
        //get and disable player controller
        currentPlayerController = user.GetComponent<BasicController>();
        currentPlayerController.canMove = false;

        OnActivateStart();
    }
    public sealed override void OnActivateUp(Interactor user)
    {
        OnActivateStop();

        isActive = false;
        currentUser = null;
        //enable the player controller
        currentPlayerController.canMove = true;
        currentPlayerController = null;
    }

    protected virtual void OnActivateStart() { }
    protected virtual void OnActivateStop() { }
}
