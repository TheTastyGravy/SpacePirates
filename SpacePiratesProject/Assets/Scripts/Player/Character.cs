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

    protected override void Start()
    {
        base.Start();

        m_Rigidbody = GetComponent< Rigidbody >();
        IsKinematic = true;
        enabled = false;
    }


    void FixedUpdate()
    {
        Player.GetInput( Player.Control.LEFT_STICK, out Vector3 input );

        Vector3 movement;
        if (Camera.main != null)
		{
            // Get camera directions on the Y plane
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            // Calculate movement using the camera
            movement = camForward.normalized * input.z + camRight.normalized * input.x;
        }
        else
		{
            movement = input;
		}

        movement *= MoveSpeed;
        // Set velocity directly, keeping the y component when its negitive
        m_Rigidbody.velocity = new Vector3(movement.x, Mathf.Min(m_Rigidbody.velocity.y, 0), movement.z);

		// Rotate over time towards the direction of movement
		Vector3 forward = Vector3.Slerp( transform.forward, movement, Time.fixedDeltaTime * TurnSpeed );
		Quaternion quat = Quaternion.FromToRotation( transform.forward, forward );
		m_Rigidbody.MoveRotation( m_Rigidbody.rotation * quat );

		// Set animator value
        currentCharacter.animator.SetFloat("Speed", movement.magnitude);
    }

	void OnDisable()
	{
        m_Rigidbody.velocity = Vector3.zero;
        currentCharacter.animator.SetFloat("Speed", 0);
    }


	private Rigidbody m_Rigidbody;
}