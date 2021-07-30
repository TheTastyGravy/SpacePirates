using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent( typeof( Rigidbody ) ) ]
public class PlayerController : ICharacter
{
    public float MoveSpeed = 1;
    public float TurnSpeed = 10;

    void Start()
    {
        m_Rigidbody = GetComponent< Rigidbody >();
        m_Interactor = GetComponent< Interactor >();
        Player.AddInputListener( Player.Control.A_PRESSED, OnAPressed );
    }

    private void OnDestroy()
    {
        Player.RemoveInputListener( Player.Control.A_PRESSED, OnAPressed );
    }

    void FixedUpdate()
    {
        Player.GetInput( Player.Control.LEFT_STICK, out Vector3 movement );

        // Move using the rigidbody
        m_Rigidbody.MovePosition( m_Rigidbody.position + MoveSpeed * Time.fixedDeltaTime * movement );

		// Rotate over time towards the direction of movement
		Vector3 forward = Vector3.Slerp( transform.forward, movement, Time.fixedDeltaTime * TurnSpeed );
		Quaternion quat = Quaternion.FromToRotation( transform.forward, forward );
		m_Rigidbody.MoveRotation( m_Rigidbody.rotation * quat );
    }

    private void OnAPressed( InputAction.CallbackContext _ )
    {
        m_Interactor?.Interact();
    }

    private Rigidbody m_Rigidbody;
    private Interactor m_Interactor;
}