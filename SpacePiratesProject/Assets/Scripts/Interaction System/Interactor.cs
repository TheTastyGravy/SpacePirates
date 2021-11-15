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
		GameManager.OnStartTransition += ExitGame;
		GameManager.OnEndTransition += EnterGame;
	}

	void OnDestroy()
	{
		GameManager.OnStartTransition -= ExitGame;
		GameManager.OnEndTransition -= EnterGame;

		if (player != null)
		{
			InputAction aAction = Player.GetInputAction(Player.Control.A_PRESSED);
			aAction.started -= OnInteractionInput;
			aAction.canceled -= OnInteractionInput;
			InputAction bAction = Player.GetInputAction(Player.Control.B_PRESSED);
			bAction.started -= OnInteractionInput;
			bAction.canceled -= OnInteractionInput;
		}
	}
	
	public void EnterGame(Scene scene, GameManager.GameState otherScene)
	{
		if (scene.name != "GAME")
			return;

		InteractionManager.Instance.interactors.Add(this);
		grabAttach = player.Character.GetCharacter().grabTransform;
		// Setup input callbacks
		InputAction aAction = Player.GetInputAction(Player.Control.A_PRESSED);
		aAction.started += OnInteractionInput;
		aAction.canceled += OnInteractionInput;
		InputAction bAction = Player.GetInputAction(Player.Control.B_PRESSED);
		bAction.started += OnInteractionInput;
		bAction.canceled += OnInteractionInput;

		enabled = true;
	}

	public void ExitGame(Scene scene, GameManager.GameState otherScene)
	{
		if (scene.name != "GAME")
			return;
		
		// Menu spam fix
		if (InteractionManager.Instance != null)
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
		if (IsInteracting)
        {
			currentInteractable.ButtonUp(this);
			EndInteraction();
		}
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

	internal Interactable FindClosestUsableInteractable(Player.Control control, bool updateManager = false)
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

			if (updateManager && sqrDist < obj.interactionRadius * obj.interactionRadius && !InteractionManager.Instance.interactablesInRange.Contains(obj))
			{
				InteractionManager.Instance.interactablesInRange.Add(obj);
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

		heldGrabbable.attach.SetParent(Player.transform.parent);
		heldGrabbable.attach.SetPositionAndRotation(transform.position + transform.rotation * dropPositionOffset, transform.rotation);
		heldGrabbable.Drop(this);
		heldGrabbable.rb.AddForceAtPosition(transform.forward * 1.2f, heldGrabbable.rb.position + UnityEngine.Random.insideUnitSphere * 0.15f, ForceMode.Impulse);
		heldGrabbable = null;
		dropButton = 0;
	}
}
