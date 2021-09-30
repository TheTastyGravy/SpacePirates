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

	[Space]
	public TextMeshProUGUI currentEventText;
	private string baseText = "Current Event: ";



	public void StartEvent(Level.Event newEvent)
	{
		currentEventText.text = baseText + newEvent.type.ToString();

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
	}

    public void StopEvent()
	{
		if (currentEvent != null)
		{
			currentEvent.Stop();
			currentEvent = null;

			currentEventText.text = baseText + "null";
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
