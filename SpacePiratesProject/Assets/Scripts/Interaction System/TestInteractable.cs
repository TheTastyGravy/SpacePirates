using UnityEngine;

public class TestInteractable : IInteractable
{
	public override void Activate(Interactor user)
	{
		Debug.Log(gameObject.name + " has been interacted with by " + user.gameObject.name);
	}
}
