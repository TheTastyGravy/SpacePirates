using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public GameObject prompt;

    // The number of players inside this trigger
    private int playerCount = 0;



    public abstract void Activate(Interactor user);


    protected virtual void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
		{
            playerCount++;
            if (prompt != null)
                prompt.SetActive(true);
        }
	}
    protected virtual void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Player"))
        {
            playerCount--;
        }

        if (playerCount == 0 && prompt != null)
		{
            prompt.SetActive(false);
		}
    }
}
