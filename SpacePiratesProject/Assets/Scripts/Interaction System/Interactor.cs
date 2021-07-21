using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    // The interactable that is currently selected
    private IInteractable selected = null;


	//for now triggers are used to determine if something can be interacted with, 
	//but this could be changed to use the direction the player is facing and the 
	//distance from interactables instead, which would be better for performance and 
	//might be better for gameplay

	void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out IInteractable interactable))
		{
			selected = interactable;
			selected.HightlightObject();
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (selected != null && selected.gameObject == other.gameObject)
		{
			selected.UnhighlightObject();
			selected = null;
		}
	}


	public void InteractDown()
	{
		// Nothing is selected, do nothing
		if (selected == null)
			return;
		
		selected.OnActivateDown(this);
	}

	public void InteractUp()
	{
		// Nothing is selected, do nothing
		if (selected == null)
			return;
		
		selected.OnActivateUp(this);
	}
}
