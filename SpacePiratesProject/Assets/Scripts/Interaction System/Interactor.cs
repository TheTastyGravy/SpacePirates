using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
	public Vector3 dropPositionOffset;

	private Transform grabAttach;
	private Player player;
	public Player Player => player;

	private Interactable currentInteractable = null;
	private Player.Control controlUsed;
	public bool IsInteracting => currentInteractable != null;

	private Grabbable heldGrabbable = null;
	private Player.Control dropButton;
	public Grabbable HeldGrabbable => heldGrabbable;



	void Awake()
	{
		player = GetComponentInParent<Player>();
		SceneManager.activeSceneChanged += OnSceneChanged;
	}

	private void OnSceneChanged(Scene current, Scene next)
	{
		if (current.name == "GAME")
		{
			ExitGame();
		}
		else if (next.name == "GAME")
		{
			// Wait a frame
			Invoke(nameof(EnterGame), 0);
		}
	}
	
	public void EnterGame()
	{
		InteractionManager.Instance.interactors.Add(this);
		grabAttach = GetComponent<ICharacter>().GetCharacter().grabTransform;
		// Setup input callbacks
		InputAction aAction = Player.GetInputAction(Player.Control.A_PRESSED);
		aAction.started += OnInteractionInput;
		aAction.canceled += OnInteractionInput;
		InputAction bAction = Player.GetInputAction(Player.Control.B_PRESSED);
		bAction.started += OnInteractionInput;
		bAction.canceled += OnInteractionInput;

		enabled = true;
	}

	public void ExitGame()
	{
		InteractionManager.Instance.interactors.Remove(this);
		// Remove input callbacks
		InputAction aAction = Player.GetInputAction(Player.Control.A_PRESSED);
		aAction.started -= OnInteractionInput;
		aAction.canceled -= OnInteractionInput;
		InputAction bAction = Player.GetInputAction(Player.Control.B_PRESSED);
		bAction.started -= OnInteractionInput;
		bAction.canceled -= OnInteractionInput;

		Drop();
		enabled = false;
	}

	void OnDisable()
	{
		EndInteraction();
	}

	private void OnInteractionInput(InputAction.CallbackContext context)
	{
		if (!isActiveAndEnabled)
			return;

		// Currently the only interaction buttons are A and B, so this works despite being dumb
		Player.Control control = context.action == Player.GetInputAction(Player.Control.A_PRESSED) ? Player.Control.A_PRESSED : Player.Control.B_PRESSED;

		if (context.started)
		{
			if (IsInteracting)
			{
				if (controlUsed == control)
				{
					currentInteractable.ButtonDown(this);
				}

				return;
			}

			Interactable interactable = FindClosestUsableInteractable(control);
			if (interactable != null)
			{
				if (interactable is Grabbable)
				{
					Pickup(interactable as Grabbable);
				}
				else
				{
					currentInteractable = interactable;
					controlUsed = control;

					player.Character.enabled = false;
					(player.Character as Character).IsKinematic = true;
					currentInteractable.StartInteraction(this);
				}
			}
			// If we found nothing to interact with, check if we can drop a held grabbable
			else if (heldGrabbable != null && dropButton == control)
			{
				Drop();
			}
		}
		else //context.ended
		{
			if (!IsInteracting || controlUsed != control)
				return;

			currentInteractable.ButtonUp(this);
		}
	}

	internal Interactable FindClosestUsableInteractable(Player.Control control)
	{
		float closestSqrDist = float.PositiveInfinity;
		Interactable interactable = null;
		foreach (var obj in InteractionManager.Instance.interactables)
		{
			// If we cant use the interactable, continue
			if (!obj.CanBeUsed(this, control))
			{
				continue;
			}

			Vector3 diff = obj.InteractionCenter - transform.position;
			// Ignore height
			diff.y = 0;
			float sqrDist = diff.sqrMagnitude;
			if (sqrDist < obj.interactionRadius * obj.interactionRadius && sqrDist < closestSqrDist)
			{
				closestSqrDist = sqrDist;
				interactable = obj;
			}
		}

		return interactable;
	}

	/// <summary>
	/// End an ongoing interaction
	/// </summary>
	public void EndInteraction()
	{
		if (!IsInteracting)
			return;

		currentInteractable.StopInteraction(this);
		player.Character.enabled = true;
		(player.Character as Character).IsKinematic = false;

		currentInteractable = null;
	}

	/// <summary>
	/// Pickup a grabbable item
	/// </summary>
	public void Pickup(Grabbable grabbable)
	{
		if (heldGrabbable != null && !grabbable.IsBeingUsed)
			return;

		heldGrabbable = grabbable;
		dropButton = heldGrabbable.dropButton;
		heldGrabbable.attach.SetParent(grabAttach);
		heldGrabbable.attach.SetPositionAndRotation(grabAttach.position, grabAttach.rotation);
		heldGrabbable.Pickup(this);
	}

	/// <summary>
	/// Drop the currently held item
	/// </summary>
	public void Drop()
	{
		if (heldGrabbable == null)
			return;

		heldGrabbable.attach.SetParent(null);
		heldGrabbable.attach.SetPositionAndRotation(transform.position + transform.rotation * dropPositionOffset, transform.rotation);
		heldGrabbable.Drop(this);
		heldGrabbable = null;
		dropButton = 0;
	}
}
