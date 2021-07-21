using UnityEngine;

public class TestInteractable : IInteractable
{
	public override void OnActivateDown(Interactor user)
	{
		Debug.Log(gameObject.name + " has been interacted with by " + user.gameObject.name + "\nDown event");
	}
	public override void OnActivateUp(Interactor user)
	{
		Debug.Log(gameObject.name + " has been interacted with by " + user.gameObject.name + "\nUp event");
	}
}
