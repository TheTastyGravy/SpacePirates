using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent< Interactor > Selected;
    public UnityEvent< Interactor > Deselected;
    public UnityEvent< InteractionCallback > Interacted;

    public Interactor LastInteractedBy => m_LastInteractedBy;
    public List< Interactor > SelectedBy
    {
        get
        {
            List< Interactor > selectedBy = new List< Interactor >();

            foreach ( Interactor interactor in m_InteractorsActive )
            {
                if ( ReferenceEquals( interactor.Selected, this ) )
                {
                    selectedBy.Add( interactor );
                }
            }

            return selectedBy;
        }
    }
    public bool IsSelected => m_SelectedCount > 0;
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

                foreach ( Interactor interactor in m_InteractorsActive )
                {
                    interactor.DeregisterInteractable( this, !m_IsActive );
                    interactor.RegisterInteractable( this, m_IsActive );
                }

                foreach ( Interactor interactor in m_InteractorsInactive )
                {
                    interactor.DeregisterInteractable( this, !m_IsActive );
                    interactor.RegisterInteractable( this, m_IsActive );
                }
            }
        }
    }

    public bool Interact( Interactor a_Interactor )
    {
        if ( !IsActive || a_Interactor == null )
        {
            return false;
        }

        m_LastInteractedBy = a_Interactor;
        OnInteract( new InteractionCallback( a_Interactor, this ) );
        Interacted.Invoke( new InteractionCallback( a_Interactor, this ) );
        return true;
    }
    
    internal void RegisterInteractor( Interactor a_Interactor, bool a_Active )
    {
        if ( a_Active )
        {
            m_InteractorsActive.Add( a_Interactor );
            ++m_SelectedCount;
            OnSelected( a_Interactor );
            Selected.Invoke( a_Interactor );
        }
        else
        {
            m_InteractorsInactive.Add( a_Interactor );
        }
    }

    internal void DeregisterInteractor( Interactor a_Interactor, bool a_Active )
    {
        if ( a_Active )
        {
            bool wasSelected = a_Interactor.Selected == this;
            m_InteractorsActive.Remove( a_Interactor );
            m_SelectedCount -= wasSelected ? 1 : 0;
            OnDeselected( a_Interactor );
            Deselected.Invoke( a_Interactor );
        }
        else
        {
            m_InteractorsInactive.Remove( a_Interactor );
        }
    }

    protected virtual void OnInteract( InteractionCallback a_Interaction ) { }
    
    protected virtual void OnSelected( Interactor a_SelectedBy ) { }

    protected virtual void OnDeselected( Interactor a_DeselectedBy ) { }

    private void Awake()
    {
        m_IsActive = m_IsActiveOnStart;
    }

    private void OnTriggerEnter( Collider a_Other )
    {
        if ( a_Other.TryGetComponent( out Interactor interactor ) )
        {
            interactor.RegisterInteractable( this, m_IsActive );
            RegisterInteractor( interactor, interactor.IsActive );
        }
    }

    private void OnTriggerExit( Collider a_Other )
    {
        if ( a_Other.TryGetComponent( out Interactor interactor ) )
        {
            DeregisterInteractor( interactor, interactor.IsActive );
            interactor.DeregisterInteractable( this, m_IsActive );
        }
    }

    private void OnDestroy()
    {
        m_InteractorsActive.ForEach( interactor => interactor.DeregisterInteractable( this, m_IsActive ) );
        m_InteractorsInactive.ForEach( interactor => interactor.DeregisterInteractable( this, m_IsActive ) );
    }

    [ SerializeField ] private bool m_IsActiveOnStart;
    private List< Interactor > m_InteractorsActive = new List< Interactor >();
    private List< Interactor > m_InteractorsInactive = new List< Interactor >();
    private Interactor m_LastInteractedBy;
    private bool m_IsActive;
    private int m_SelectedCount;
}
