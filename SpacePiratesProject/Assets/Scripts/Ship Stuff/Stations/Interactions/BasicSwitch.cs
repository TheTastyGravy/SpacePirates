using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class BasicSwitch : Interactable
{
    public BasicDelegate OnActivated;
    [Tooltip("How long the switch is disabled for after being used")]
    public float interactionCooldown = 1;

	[Tooltip("Used to reactor switches")]
	public BasicSwitch pairedSwitch;
	[Tooltip("Only used for reactor switches")]
	public Sprite onePlayerIcon;
	internal Image selectedImage;
	internal Image baseImage;
	internal Sprite buttonPrompt;
	internal Sprite baseIcon;
	[Space]
	public EventReference interactionEvent;

	[HideInInspector]
	public bool forceDisabled = false;



	void Start()
	{
		if (pairedSwitch == null)
			return;

		selectedImage = interactionPrompt.selectedImages[0];
		baseImage = interactionPrompt.baseImages[0];
		buttonPrompt = selectedImage.sprite;
		baseIcon = baseImage.sprite;
		interactionPrompt.OnSelected += OnSelected;
		interactionPrompt.OnUnselected += OnUnselected;
	}

	private void OnSelected()
	{
		// Update this prompt
		selectedImage.sprite = pairedSwitch.interactionPrompt.IsSelected ? buttonPrompt : onePlayerIcon;
		// Update the other switches prompts
		pairedSwitch.selectedImage.sprite = pairedSwitch.buttonPrompt;
		pairedSwitch.baseImage.sprite = pairedSwitch.onePlayerIcon;
		pairedSwitch.interactionPrompt.PopV2();
	}

	private void OnUnselected()
	{
		// Update the other switches prompts
		pairedSwitch.selectedImage.sprite = pairedSwitch.onePlayerIcon;
		pairedSwitch.baseImage.sprite = pairedSwitch.baseIcon;
		pairedSwitch.interactionPrompt.PopV2();
	}

	private void Reenable()
	{
		if (forceDisabled)
			return;

		enabled = true;
	}

	protected override void OnInteractionStart()
	{
        OnActivated?.Invoke();
		
		// Pop interaction prompt, and start cooldown
		interactionPrompt.Pop();
		
		currentInteractor.EndInteraction();

		enabled = false;
		Invoke(nameof(Reenable), interactionCooldown);
		// Play sound effect
		RuntimeManager.PlayOneShot(interactionEvent);
	}

	protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
	{
        button = Player.Control.A_PRESSED;
        return true;
    }
}
