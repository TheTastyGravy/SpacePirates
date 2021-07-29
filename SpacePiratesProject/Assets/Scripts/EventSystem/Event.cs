using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event
{
	public void Start()
	{
		//add to event systems list of events
		EventManager.Instance.activeEvents.Add(this);

		OnStart();
	}
	public void Stop()
	{
		//remove from event systems list of events
		EventManager.Instance.activeEvents.Remove(this);

		OnStop();
	}


	/// <summary>
	/// Returns true if this event can currently be activated
	/// </summary>
	public abstract bool CanBeActivated();
	/// <summary>
	/// Called each frame while this event is active
	/// </summary>
	public abstract void Update();

	/// <summary>
	/// Called when this event has been started
	/// </summary>
	protected abstract void OnStart();
	/// <summary>
	/// Called when this event has been stoped
	/// </summary>
	protected abstract void OnStop();
}
