using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
	public Transform grabAttach;

	// All interactables in range (registered and unregistered)
	// Destroied interactables will remove themself from this list after unregistering
	internal List<Interactable> interactables = new List<Interactable>();
	// Interactions for interactables that have been registered
	private List<Interaction> registeredInteractions = new List<Interaction>();

	private Player player;
	public Player Player => player;

	private Grabbable heldGrabbable = null;
	public Grabbable HeldGrabbable => heldGrabbable;



	void Start()
	{
		player = GetComponentInParent<Player>();
	}


	/// <summary>
	/// Register an interactable to be interacted with by a button
	/// </summary>
	public void RegisterInteractable(Interactable interactable, Player.Control button)
	{
		// Get input action for the button
		InputAction inputAction = player.GetInputAction(button);

		// Setup an interaction instance
		Interaction interaction = new Interaction();
		interaction.SetupInteraction(interactable, this, inputAction);

		registeredInteractions.Add(interaction);
	}
	/// <summary>
	/// Unregister an interactable to not be interacted with
	/// </summary>
	public void UnregisterInteractable(Interactable interactable)
	{
		// Find the interaction for the interactable and destroy it
		foreach (var interaction in registeredInteractions)
		{
			if (interaction.interactable == interactable)
			{
				interaction.DestroyInteraction();
				registeredInteractions.Remove(interaction);
				return;
			}
		}
	}

	/// <summary>
	/// Re-register all interactables
	/// </summary>
	public void UpdateRegistry()
	{
		// Destroy all interactions
		for (int i = 0; i < registeredInteractions.Count; i++)
		{
			registeredInteractions[i].DestroyInteraction();
		}
		registeredInteractions.Clear();

		// Notify all interactables to re-register
		foreach (var interactable in interactables)
		{
			interactable.Notify_Removed(this);
			interactable.Notify_Register(this);
		}
	}


	/// <summary>
	/// Pickup a grabbable item
	/// </summary>
	public void Pickup(Grabbable grabbable)
	{
		if (heldGrabbable != null)
			return;
		

		grabbable.attach.SetParent(grabAttach);
		grabbable.attach.SetPositionAndRotation(grabAttach.position, grabAttach.rotation);

		heldGrabbable = grabbable;
		UpdateRegistry();

		grabbable.Pickup(this);
	}
	/// <summary>
	/// Drop the currently held item
	/// </summary>
	public void Drop()
	{
		if (heldGrabbable == null)
			return;


		heldGrabbable.attach.SetParent(null);
		heldGrabbable.Drop(this);

		heldGrabbable = null;
		UpdateRegistry();
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Interactable interactable))
		{
			if (interactables.Contains(interactable))
				return;

			interactables.Add(interactable);
			// Notify interactable that it can register
			interactable.Notify_Register(this);
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Interactable interactable))
		{
			UnregisterInteractable(interactable);
			interactables.Remove(interactable);
			// Notify interactable that it has been removed
			interactable.Notify_Removed(this);
		}
	}
}

internal class Interaction
{
	// The interactable that this belongs to
	public Interactable interactable;

	private Interactor interactor;
	private InputAction inputAction;


	public void SetupInteraction(Interactable interactable, Interactor interactor, InputAction inputAction)
	{
		this.interactable = interactable;
		this.interactor = interactor;
		this.inputAction = inputAction;

		inputAction.started += OnInteractStarted;
		inputAction.canceled += OnInteractCanceled;
	}
	public void DestroyInteraction()
	{
		inputAction.started -= OnInteractStarted;
		inputAction.canceled -= OnInteractCanceled;
	}

	private void OnInteractStarted(InputAction.CallbackContext context)
	{
		interactable.Interaction_Start(interactor);
	}
	private void OnInteractCanceled(InputAction.CallbackContext context)
	{
		interactable.Interaction_Stop(interactor);
	}
}