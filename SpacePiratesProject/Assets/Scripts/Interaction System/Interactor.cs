using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    // The interactables we are able to use
	[HideInInspector]
    public List<Interactable> interactables = new List<Interactable>();



	void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Interactable interactable))
		{
			interactables.Add(interactable);
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Interactable interactable))
		{
			interactables.Remove(interactable);
		}
	}


	public void Interact()
	{
		if (interactables.Count == 0)
			return;


		if (interactables.Count == 1)
		{
			interactables[0].Activate(this);
		}
		else
		{
			//use closest one

			interactables[0].Activate(this);
		}
	}
}
