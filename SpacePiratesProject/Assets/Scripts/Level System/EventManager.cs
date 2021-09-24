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
	public GameObject missilePrefab;
	public float enterTime = 1;
	public float exitTime = 1;
	public float firePeriod = 1;
	public float spreadAngle = 45;
	[Space]
	[Tooltip("How often to change the wander target")]
	public float wanderFrequency = 1;
	public float wanderDist = 0.75f;
	public float wanderSpeed = 3;
	public float wanderAcceleration = 1;


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
					missilePrefab = missilePrefab,
					enterTime = enterTime,
					exitTime = exitTime,
					firePeriod = firePeriod,
					spreadAngle = spreadAngle,
					wanderFrequency = wanderFrequency,
					wanderDist = wanderDist,
					wanderSpeed = wanderSpeed,
					wanderAcceleration = wanderAcceleration
				};
				break;
		}

		currentEvent.Start();
	}

    public void StopEvent()
	{
		currentEvent.Stop();
        currentEvent = null;

		currentEventText.text = baseText + "null";
	}

    public void UpdateEvent()
	{
		currentEvent.Update();
	}
}
