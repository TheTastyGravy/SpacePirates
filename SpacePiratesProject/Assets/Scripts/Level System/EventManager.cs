using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventManager : Singleton<EventManager>
{
	[Header("Astroid Field")]
	public float timeBetweenWaves = 2;
	public int astroidsPerWave = 4;


	[Header("Plasma Storm")]
	public float timeBetweenDamage = 1;


	[Header("Ship Attack")]
	public GameObject shipPrefab;
	public int shipHealth = 10;


	private Event currentEvent;
	public delegate void EventDelegate(Level.Event.Type eventType);
	public EventDelegate OnEventChange;



	public void StartEvent(Level.Event newEvent)
	{
		// Create event, passing relevent paramiters
		switch (newEvent.type)
		{
			case Level.Event.Type.AstroidField:
				currentEvent = new AstroidField()
				{
					timeBetweenWaves = timeBetweenWaves,
					astroidsPerWave = astroidsPerWave
				};
				break;
			case Level.Event.Type.PlasmaStorm:
				currentEvent = new PlasmaStorm()
				{
					timeBetweenDamage = timeBetweenDamage
				};
				break;
			case Level.Event.Type.ShipAttack:
				currentEvent = new ShipAttack()
				{
					shipPrefab = shipPrefab,
					shipHealth = shipHealth
				};
				break;
		}

		currentEvent.Start();
		OnEventChange?.Invoke(newEvent.type);
	}

    public void StopEvent()
	{
		if (currentEvent != null)
		{
			OnEventChange?.Invoke(Level.Event.Type.None);
			currentEvent.Stop();
			currentEvent = null;
		}
	}

    public void UpdateEvent()
	{
		if (currentEvent != null)
		{
			currentEvent.Update();
		}
	}
}
