using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AstroidLogic : MonoBehaviour
{
	public UnityEvent onContact;
	private bool hasContacted = false;

	void OnCollisionEnter(Collision collision)
	{
		if (!hasContacted)
		{
			onContact.Invoke();
			hasContacted = true;
		}
	}
}
