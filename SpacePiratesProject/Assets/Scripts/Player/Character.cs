using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent( typeof( Rigidbody ) ) ]
public class Character : ICharacter
{
    [ Header( "Movement" ) ]
    public float MoveSpeed = 1;
    public float TurnSpeed = 10;

    public float Health => m_Health;
    public float HealthMax => m_HealthMax;
    public float HealthOnRevive => m_HealthOnRevive;
    public bool IsDead => m_Health <= 0;
    public bool IsAlive => m_Health > 0;
    public bool IsKinematic
    {
        get
        {
            return !m_Rigidbody.isKinematic;
        }
        set
        {
            m_Rigidbody = m_Rigidbody ?? GetComponent< Rigidbody >();
            m_Rigidbody.isKinematic = !value;
        }
    }

    void Start()
    {
        m_Rigidbody = GetComponent< Rigidbody >();
        m_Interactor = GetComponent< Interactor >();
        m_ReviveStation = GetComponent< ReviveStation >();
        Player.AddInputListener( Player.Control.A_PRESSED, OnAPressed );
    }

    private void OnDestroy()
    {
        Player.RemoveInputListener( Player.Control.A_PRESSED, OnAPressed );
    }

    void FixedUpdate()
    {
        Player.GetInput( Player.Control.LEFT_STICK, out Vector3 movement );

        // Move using the rigidbody, constraining the y component
		Vector3 newPos = m_Rigidbody.position + MoveSpeed * Time.fixedDeltaTime * movement;
		newPos.y = 0;
		m_Rigidbody.MovePosition( newPos );

		// Rotate over time towards the direction of movement
		Vector3 forward = Vector3.Slerp( transform.forward, movement, Time.fixedDeltaTime * TurnSpeed );
		Quaternion quat = Quaternion.FromToRotation( transform.forward, forward );
		m_Rigidbody.MoveRotation( m_Rigidbody.rotation * quat );
    }

    public void ApplyHealthModifier( float a_HealthModifier )
    {
        if ( m_Health == 0 )
        {
            return;
        }

        m_Health += a_HealthModifier;

        if ( m_Health <= 0 )
        {
            enabled = false;
            m_ReviveStation.SetIsUsable( true );
        }

        m_Health = Mathf.Clamp( m_Health, 0.0f, m_HealthMax );
        HUDController.GetPlayerCard( Player.Slot ).HealthBar.Value = m_Health;
    }

    public void Revive()
    {
        m_Health = m_HealthOnRevive;
        enabled = true;
        m_ReviveStation.SetIsUsable( false );
    }

    private void OnAPressed( InputAction.CallbackContext _ )
    {
        m_Interactor?.Interact();
    }

    private Rigidbody m_Rigidbody;
    private Interactor m_Interactor;
    private ReviveStation m_ReviveStation;

    [ Header( "Health" ) ]
    [ SerializeField ] [ Range( 0.0f, 100.0f ) ] private float m_Health;
    [ SerializeField ] [ Range( 0.0f, 100.0f ) ] private float m_HealthMax;
    [ SerializeField ] [ Range( 0.0f, 100.0f ) ] private float m_HealthOnRevive;
}