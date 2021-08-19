using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ RequireComponent( typeof( Rigidbody ) ) ]
public class Character : ICharacter
{
    [ Header( "Movement" ) ]
    public float MoveSpeed = 1;
    public float TurnSpeed = 10;

	private Animator animator;

    public bool IsKinematic
    {
        get
        {
            return !m_Rigidbody.isKinematic;
        }
        set
        {
            m_Rigidbody = m_Rigidbody ?? GetComponent< Rigidbody >();
            m_Rigidbody.isKinematic = value;
        }
    }

    void Start()
    {
        m_Rigidbody = GetComponent< Rigidbody >();
		animator = GetComponent<Animator>();
        IsKinematic = true;
        enabled = false;
    }


    void FixedUpdate()
    {
        Player.GetInput( Player.Control.LEFT_STICK, out Vector3 movement );

        // Move using the rigidbody, constraining the y component
		Vector3 newPos = m_Rigidbody.position + MoveSpeed * Time.fixedDeltaTime * movement;
		m_Rigidbody.MovePosition( newPos );

		// Rotate over time towards the direction of movement
		Vector3 forward = Vector3.Slerp( transform.forward, movement, Time.fixedDeltaTime * TurnSpeed );
		Quaternion quat = Quaternion.FromToRotation( transform.forward, forward );
		m_Rigidbody.MoveRotation( m_Rigidbody.rotation * quat );

		// Set animator value
		if (animator != null)
		{
			animator.SetFloat("Speed", movement.magnitude);
		}
    }


    private Rigidbody m_Rigidbody;
}