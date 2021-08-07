using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabInteractor : Interactor
{
    public Transform Attach;
    public bool DropOnInteract;

    public Grabbable Grabbed => m_Grabbed;

    protected override void OnInteract( InteractionCallback a_Interaction )
    {
        if ( a_Interaction.Interactable == null )
        {
            if ( DropOnInteract )
            {
                Drop();
            }

            return;
        }

        if ( a_Interaction.Interactable is Grabbable grabbable )
        {
            PickUp( grabbable );
        }
    }

    public bool PickUp( Grabbable a_Grabbable )
    {
        if ( a_Grabbable == null )
        {
            return false;
        }

        Drop();
        a_Grabbable.DeregisterInteractor( this, IsActive );
        DeregisterInteractable( a_Grabbable, a_Grabbable.IsActive );
        m_Grabbed = a_Grabbable;
        m_Grabbed.Attach?.SetParent( Attach );
        m_Grabbed.Attach?.SetPositionAndRotation( Attach.position, Attach.rotation );
        m_Grabbed.OnPickup( this );
        return true;
    }

    public void Drop()
    {
        if ( m_Grabbed == null )
        {
            return;
        }

        m_Grabbed.Attach?.SetParent( null );
        m_Grabbed.OnDrop( this );
        m_Grabbed = null;
    }

    private Grabbable m_Grabbed;
}
