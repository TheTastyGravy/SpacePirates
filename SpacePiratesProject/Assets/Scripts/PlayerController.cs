using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float turnSpeed = 10;

    private Rigidbody rb;
    private Interactor interactor;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        interactor = GetComponent<Interactor>();
    }

    void FixedUpdate()
    {
        Vector3 movement = GetMovement();
        // Move using the rigidbody
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);
        // Rotate over time towards the direction of movement
        Quaternion rotToDir = Quaternion.FromToRotation(transform.forward, movement);
        rb.MoveRotation(rb.rotation * Quaternion.Lerp(Quaternion.identity, rotToDir, Time.deltaTime * turnSpeed));

        // Interaction
        if (CheckInteract())
		{
            interactor.Interact();
		}
    }

    // Read movement input
    private Vector3 GetMovement()
    {
        // Keyboard
        if (Keyboard.current != null)
		{
            var keyboard = Keyboard.current;

            Vector3 movement = Vector3.zero;
            movement.x += keyboard.dKey.ReadValue();
            movement.x -= keyboard.aKey.ReadValue();
            movement.z += keyboard.wKey.ReadValue();
            movement.z -= keyboard.sKey.ReadValue();
            return movement;
        }


        // Fallback
        return Vector3.zero;
    }

    // Read interaction input
    private bool CheckInteract()
	{
        // Keyboard
        if (Keyboard.current != null)
        {
            var keyboard = Keyboard.current;
            return keyboard.eKey.wasPressedThisFrame;
        }


        // Fallback
        return false;
    }
}