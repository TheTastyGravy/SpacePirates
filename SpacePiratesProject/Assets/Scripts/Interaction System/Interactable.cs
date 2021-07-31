using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public GameObject prompt;

	public Collider _collider;

	private List<Interactor> interactors = new List<Interactor>();
    private bool isUseable = true;

    public void Activate(Interactor user)
	{
        if (isUseable)
		{
            OnActivate(user);
		}
	}
    protected abstract void OnActivate(Interactor user);


    protected virtual void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
		{
			interactors.Add(other.GetComponent<Interactor>());

			if (isUseable && prompt != null)
                prompt.SetActive(true);
        }
	}
    protected virtual void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Player"))
        {
			interactors.Remove(other.GetComponent<Interactor>());
		}

        if (isUseable && interactors.Count == 0 && prompt != null)
		{
            prompt.SetActive(false);
		}
    }

    public void SetIsUsable(bool value)
	{
        isUseable = value;
		_collider.enabled = value;

		if (!isUseable)
		{
			foreach (var obj in interactors)
			{
				obj.interactables.Remove(this);
			}
		}
		
		// Update prompt
		if (isUseable && interactors.Count > 0 && prompt != null)
		{
            prompt.SetActive(true);
        }
        else if (!isUseable && prompt != null)
		{
            prompt.SetActive(false);
        }
    }

	void OnDestroy()
	{
		//remove this from interactors
		foreach (var obj in interactors)
		{
			obj.interactables.Remove(this);
		}
	}
}
