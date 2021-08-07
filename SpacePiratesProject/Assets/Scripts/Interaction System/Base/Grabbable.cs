using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : Interactable
{
    public Transform Attach;
    public Rigidbody Rigidbody;
    public GrabInteractor GrabInteractor;

    public virtual void OnPickup( GrabInteractor a_GrabInteractor )
    {
        Rigidbody.isKinematic = true;
        Rigidbody.detectCollisions = false;
    }

    public virtual void OnDrop( GrabInteractor a_GrabInteractor )
    {
        Rigidbody.isKinematic = false;
        Rigidbody.detectCollisions = true;
    }

    protected override void OnInteract( InteractionCallback a_Interaction )
    {
        if ( a_Interaction.Interactor is GrabInteractor grabInteractor )
        {
            m_GrabInteractor = grabInteractor;
        }
    }
    
    private GrabInteractor m_GrabInteractor;
}