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



	protected override void Awake()
	{
		base.Awake();
		rb = GetComponent<Rigidbody>();
		if (attach == null)
			attach = transform;
	}

	protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
	{
		button = pickupButton;
		return interactor.HeldGrabbable == null && !IsBeingUsed;
	}

	internal void Pickup(Interactor interactor)
	{
		if (IsBeingUsed)
			return;

		currentInteractor = interactor;
		rb.isKinematic = true;
		rb.detectCollisions = false;
		enabled = false;
		OnPickup();
	}

	internal void Drop(Interactor interactor)
	{
		if (!IsBeingUsed || interactor != currentInteractor)
			return;

		rb.isKinematic = false;
		rb.detectCollisions = true;
		enabled = true;
		OnDrop();
		currentInteractor = null;
	}

	protected virtual void OnPickup() { }
	protected virtual void OnDrop() { }
}
