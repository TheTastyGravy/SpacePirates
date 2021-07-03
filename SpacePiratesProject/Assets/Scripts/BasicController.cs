using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BasicController : MonoBehaviour
{
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

		// Basic movement with WASD
		float xVal = 0, yVal = 0;
		xVal += keyboard.dKey.ReadValue();
		xVal -= keyboard.aKey.ReadValue();
		yVal += keyboard.wKey.ReadValue();
		yVal -= keyboard.sKey.ReadValue();
		rb.MovePosition(rb.position + new Vector3(xVal, 0, yVal) * Time.deltaTime * speed);

		// Interact with E
		if (interactor != null && keyboard.eKey.wasPressedThisFrame)
		{
			interactor.Interact();
		}
	}
}
