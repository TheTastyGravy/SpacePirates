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
        Player.AddInputListener( Player.Control.RIGHT_BUMPER_PRESSED, OnRBumperPressed );
    }

    private void OnDestroy()
    {
        Player.RemoveInputListener( Player.Control.RIGHT_BUMPER_PRESSED, OnRBumperPressed );
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

    private void OnRBumperPressed( InputAction.CallbackContext _ )
    {
        m_Interactor?.Interact();
    }

	// Read movement input
	//private Vector3 GetMovement()
 //   {
 //       // Keyboard
 //       if (Keyboard.current != null)
	//	{
 //           var keyboard = Keyboard.current;

 //           Vector3 movement = Vector3.zero;
 //           movement.x += keyboard.dKey.ReadValue();
 //           movement.x -= keyboard.aKey.ReadValue();
 //           movement.z += keyboard.wKey.ReadValue();
 //           movement.z -= keyboard.sKey.ReadValue();
 //           return movement;
 //       }
	//	// Gamepad
	//	else
	//	{
	//		Player.GetInput(Player.Control.LEFT_STICK, out Vector2 value);
	//		return value;
	//	}


 //       // Fallback
 //       return Vector3.zero;
 //   }

    // Read interaction input
 //   private bool CheckInteract()
	//{
 //       // Keyboard
 //       if (Keyboard.current != null)
 //       {
 //           var keyboard = Keyboard.current;
 //           return keyboard.eKey.wasPressedThisFrame;
 //       }
	//	// Gamepad
	//	else
	//	{
	//		return Player.GetInput(Player.Control.A_PRESSED);
	//	}

 //       // Fallback
 //       return false;
 //   }

    private Rigidbody m_Rigidbody;
    private Interactor m_Interactor;
}