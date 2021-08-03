using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    // The interactables we are able to use
	[HideInInspector]
    public List<Interactable> interactables = new List<Interactable>();

	private bool isActive = true;
	public bool IsActive { get => isActive; }


    void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Interactable interactable))
		{
			interactables.Add(interactable);
			Vector2 promptPosition = Camera.main.WorldToScreenPoint( interactables[ 0 ].prompt.transform.position );
			HUDController.ShowInteractPrompt( interactables[ 0 ], promptPosition );
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Interactable interactable))
		{
			Interactable first = interactables[ 0 ];

			if ( interactables.Count == 1 )
            {
				HUDController.HideInteractPrompt( first );
            }

			interactables.Remove(interactable);
		}
	}

	public void Interact()
	{
		if (!isActive || interactables.Count == 0)
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

	public void SetIsActive(bool value)
	{
		isActive = value;

		foreach (var obj in interactables)
		{
			if (value)
			{
				obj.interactors.Add(this);
				obj.SetIsUsable(true);
			}
			else
			{
				obj.interactors.Remove(this);
				obj.SetIsUsable(true);
			}
		}
	}
}
