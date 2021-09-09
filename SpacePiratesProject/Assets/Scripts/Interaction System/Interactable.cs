using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	public InteractionPromptLogic interactionPrompt;

	// All interactors we are in range of (registered and unregistered)
	private List<Interactor> interactors = new List<Interactor>();
	// Only interactors we have registered with
	private List<Interactor> registeredInteractors = new List<Interactor>();



	// Called when we have the option of registering with an interactor
	internal void Notify_Register(Interactor interactor)
	{
		if (!interactors.Contains(interactor))
			interactors.Add(interactor);
		
		// Derived logic determines if we should register and what button to use
		if (enabled && ShouldRegister(interactor, out Player.Control button))
		{
			if (!interactor.RegisterInteractable(this, button))
			{
				// The interaction failed to register
				return;
			}

			if (registeredInteractors.Count == 0)
			{
				Selection_Start();
			}
			if (!registeredInteractors.Contains(interactor))
				registeredInteractors.Add(interactor);
		}
	}
	// Called when we have been removed from an interactor (not just unregistered)
	internal void Notify_Removed(Interactor interactor)
	{
		interactors.Remove(interactor);
		registeredInteractors.Remove(interactor);

		if (registeredInteractors.Count == 0)
		{
			Selection_Stop();
		}
	}

    internal void Interaction_Start(Interactor interactor)
	{
		if (interactionPrompt != null)
		{
			interactionPrompt.InteractStart();
		}

		OnInteractStart(interactor);
	}
	internal void Interaction_Stop(Interactor interactor)
	{
		if (interactionPrompt != null)
		{
			interactionPrompt.InteractStop();
		}

		OnInteractStop(interactor);
	}

	private void Selection_Start()
	{
		if (interactionPrompt != null)
		{
			interactionPrompt.SelectStart();
		}

		OnSelectStart();
	}
	private void Selection_Stop()
	{
		if (interactionPrompt != null)
		{
			interactionPrompt.SelectStop();
		}

		OnSelectStop();
	}


	/// <summary>
	/// Unregister and attempt to reregister with all interactors
	/// </summary>
	protected void ReregisterInteractions()
	{
		// Unregister from all interactors
		foreach (var interactor in registeredInteractors)
		{
			interactor.UnregisterInteractable(this);
		}
		registeredInteractors.Clear();

		Selection_Stop();

		// Attempt to register with all interactors
		foreach (var interactor in interactors)
		{
			Notify_Register(interactor);
		}
	}


	/// <summary>
	/// Should we register with the interactor?
	/// </summary>
	/// <param name="interactor">The interactor to register with</param>
	/// <param name="button">The button that the interactor will use to interact</param>
	protected abstract bool ShouldRegister(Interactor interactor, out Player.Control button);
	protected virtual void OnInteractStart(Interactor interactor) { }
	protected virtual void OnInteractStop(Interactor interactor) { }
	protected virtual void OnSelectStart() { }
	protected virtual void OnSelectStop() { }


	void OnEnable()
	{
		if (interactionPrompt != null)
			interactionPrompt.enabled = true;

		// Attempt to register with all interactors
		foreach (var interactor in interactors)
		{
			Notify_Register(interactor);
		}
	}

	void OnDisable()
	{
		// Unregister from all interactors
		foreach (var interactor in registeredInteractors)
		{
			interactor.UnregisterInteractable(this);
		}
		registeredInteractors.Clear();

		Selection_Stop();
		if (interactionPrompt != null)
			interactionPrompt.enabled = false;
	}

	void OnDestroy()
	{
		// Unregister and remove this from all interactors
		foreach (var interactor in interactors)
		{
			interactor.UnregisterInteractable(this);
			interactor.interactables.Remove(this);
		}
	}
}
