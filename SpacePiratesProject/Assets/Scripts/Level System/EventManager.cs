using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
	[Header("Astroid Field")]
	public float timeBetweenWaves = 2;
	public int astroidsPerWave = 4;


	[Header("Plasma Storm")]
	public float timeBetweenDamage = 1;


	[Header("Ship Attack")]
	public GameObject shipPrefab;


	private Event currentEvent;



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
					
				};
				break;
		}

		currentEvent.Start();
	}

    public void StopEvent()
	{
		currentEvent.Stop();
        currentEvent = null;
	}

    public void UpdateEvent()
	{
		currentEvent.Update();
	}
}