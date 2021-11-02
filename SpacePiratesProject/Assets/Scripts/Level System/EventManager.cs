using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
	private float timeBetweenWaves;
	private int astroidsPerWave;

	private float timeBetweenDamage;

	public GameObject shipPrefab;
	private int shipHealth;
	private float firePeriod;

	private Event currentEvent;
	public delegate void EventDelegate(Level.Event.Type eventType);
	public EventDelegate OnEventChange;



	void Start()
	{
		// Get event values from difficulty settings
		LevelDificultyData.DiffSetting settings = GameManager.GetDifficultySettings();
		timeBetweenWaves = settings.timeBetweenAsteroidWaves;
		astroidsPerWave = settings.asteroidsPerWave;
		timeBetweenDamage = settings.timeBetweenStormDamage;
		shipHealth = settings.shipHealth;
		firePeriod = settings.shipFirePeriod;
	}

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
					shipHealth = shipHealth,
					firePeriod = firePeriod
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
