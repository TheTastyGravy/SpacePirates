using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Interactable Selected
    {
        get
        {
            return m_InteractablesActive.Count > 0 ? m_InteractablesActive[ m_InteractablesActive.Count - 1 ] : null;
        }
    }
    public bool IsActive
    {
        get
        {
            return m_IsActive;
        }
        set
        {
            if ( m_IsActive != value )
            {
                m_IsActive = value;

                foreach ( Interactable interactable in m_InteractablesActive )
                {
                    interactable.DeregisterInteractor( this, !m_IsActive );
                    interactable.RegisterInteractor( this, m_IsActive );
                }

                foreach ( Interactable interactable in m_InteractablesInactive )
                {
                    interactable.DeregisterInteractor( this, !m_IsActive );
                    interactable.RegisterInteractor( this, m_IsActive );
                }
            }
        }
    }

    public void Interact()
    {
        Interactable selected = Selected;

        if ( selected != null )
        {
            selected.Interact( this );
        }

        OnInteract( new InteractionCallback( this, selected ) );
    }
    
    internal void RegisterInteractable( Interactable a_Interactable, bool a_Active )
    {
        if ( a_Active )
        {
            if ( m_InteractablesActive.Count > 0 )
            {
                OnDeselect( Selected );
            }

            m_InteractablesActive.Add( a_Interactable );
            OnSelect( a_Interactable );
        }
        else
        {
            m_InteractablesInactive.Add( a_Interactable );
        }
    }

    internal void DeregisterInteractable( Interactable a_Interactable, bool a_Active )
    {
        if ( a_Active )
        {
            bool wasSelected = Selected == a_Interactable;
            m_InteractablesActive.Remove( a_Interactable );

            if ( wasSelected )
            {
                OnDeselect( a_Interactable );

                if ( m_InteractablesActive.Count > 0 )
                {
                    OnSelect( Selected );
                }
            }
        }
        else
        {
            m_InteractablesInactive.Remove( a_Interactable );
        }
    }

    protected virtual void OnInteract( InteractionCallback a_Interaction ) { }

    protected virtual void OnSelect( Interactable a_Interactable ) { }

    protected virtual void OnDeselect( Interactable a_Interactable ) { }

    private void Awake() => m_IsActive = m_IsActiveOnStart;

    [ SerializeField ] private bool m_IsActiveOnStart;
    private bool m_IsActive;
    private List< Interactable > m_InteractablesActive = new List< Interactable >();
    private List< Interactable > m_InteractablesInactive = new List< Interactable >();
}

public struct InteractionCallback
{
    public Interactor Interactor => m_Interactor;
    public Interactable Interactable => m_Interactable;
    public bool Succeeded
    {
        get
        {
            return m_Interactor != null && m_Interactor.IsActive && m_Interactable != null && m_Interactable.IsActive;
        }
    }

    public InteractionCallback( Interactor a_Interactor, Interactable a_Interactable )
    {
        m_Interactor = a_Interactor;
        m_Interactable = a_Interactable;
    }

    private Interactor m_Interactor;
    private Interactable m_Interactable;
}