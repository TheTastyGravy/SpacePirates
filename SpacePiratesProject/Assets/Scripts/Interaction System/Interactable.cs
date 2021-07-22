using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public GameObject prompt;

    // The number of players inside this trigger
    private int playerCount = 0;
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
            playerCount++;
            if (isUseable && prompt != null)
                prompt.SetActive(true);
        }
	}
    protected virtual void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Player"))
        {
            playerCount--;
        }

        if (isUseable && playerCount == 0 && prompt != null)
		{
            prompt.SetActive(false);
		}
    }

    public void SetIsUsable(bool value)
	{
        isUseable = value;

        // Update prompt
        if (isUseable && playerCount > 0 && prompt != null)
		{
            prompt.SetActive(true);
        }
        else if (!isUseable && prompt != null)
		{
            prompt.SetActive(false);
        }
    }
}
