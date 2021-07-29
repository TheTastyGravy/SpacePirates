using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AstroidLogic : MonoBehaviour
{
	public UnityEvent onContact;

	void OnCollisionEnter(Collision collision)
	{
		onContact.Invoke();
	}
}
