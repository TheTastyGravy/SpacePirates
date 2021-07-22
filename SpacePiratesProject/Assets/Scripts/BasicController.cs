using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BasicController : MonoBehaviour
{
	[HideInInspector]
	public bool canMove = true;

	public float speed = 1;
	private Rigidbody rb;
	private Interactor interactor;


	void Start()
	{
		rb = GetComponent<Rigidbody>();
		interactor = GetComponent<Interactor>();
	}

	void Update()
    {
		var keyboard = Keyboard.current;
		if (keyboard == null)
			return;

		// Interact with E
		if (interactor != null)
		{
			if (keyboard.eKey.wasPressedThisFrame)
			{
				interactor.Interact();
			}
		}
	}

	void FixedUpdate()
	{
		var keyboard = Keyboard.current;
		if (keyboard == null)
			return;

		// Basic movement with WASD
		float xVal = 0, yVal = 0;
		if (canMove)
		{
			xVal += keyboard.dKey.ReadValue();
			xVal -= keyboard.aKey.ReadValue();
			yVal += keyboard.wKey.ReadValue();
			yVal -= keyboard.sKey.ReadValue();
		}

		rb.MovePosition(rb.position + speed * Time.deltaTime * new Vector3(xVal, 0, yVal));
	}
}
