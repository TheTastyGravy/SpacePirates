using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : Interactable
{
	public Transform attach;
	public Player.Control pickupButton = Player.Control.A_PRESSED;
	public Player.Control dropButton = Player.Control.B_PRESSED;

	private Rigidbody rb;

	private bool isHeld = false;
	public bool IsHeld => isHeld;



	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		if (attach == null)
			attach = transform;
	}

	protected sealed override void OnInteractStart(Interactor interactor)
	{
		if (isHeld)
		{
			interactor.Drop();
		}
		else
		{
			interactor.Pickup(this);
		}
	}
	protected sealed override void OnInteractStop(Interactor interactor) { }

	protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
	{
		button = pickupButton;
		return interactor.HeldGrabbable == null && !isHeld;
	}


	internal void Pickup(Interactor interactor)
	{
		rb.isKinematic = true;
		rb.detectCollisions = false;
		isHeld = true;
		// Unregister from everything, then register to drop
		enabled = false;
		interactor.RegisterInteractable(this, dropButton);

		OnPickup(interactor);
	}
	internal void Drop(Interactor interactor)
	{
		rb.isKinematic = false;
		rb.detectCollisions = true;
		isHeld = false;
		// Unregister drop interaction, then register to everything
		interactor.UnregisterInteractable(this);
		enabled = true;

		OnDrop(interactor);
	}

	protected virtual void OnPickup(Interactor interactor) { }
	protected virtual void OnDrop(Interactor interactor) { }
}
