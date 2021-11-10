using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
	public enum EventStage
	{
		INIT,
		BEGIN,
		END
	}

	private float initTime;

	private float timeBetweenWaves;
	private int astroidsPerWave;

	private float timeBetweenDamage;

	public GameObject shipPrefab;
	private int shipHealth;
	private float firePeriod;

	private Event currentEvent;
	private Level.Event.Type currentEventType;
	private float initTimePassed;
	public delegate void EventDelegate(Level.Event.Type eventType, EventStage stage);
	public EventDelegate OnEventChange;



	void Start()
	{
		// Get event values from difficulty settings
		LevelDificultyData.DiffSetting settings = GameManager.GetDifficultySettings();
		initTime = settings.initTime;
		timeBetweenWaves = settings.timeBetweenAsteroidWaves;
		astroidsPerWave = settings.asteroidsPerWave;
		timeBetweenDamage = settings.timeBetweenStormDamage;
		shipHealth = settings.shipHealth;
		firePeriod = settings.shipFirePeriod;
	}

	public void StartEvent(Level.Event newEvent)
	{
		currentEventType = newEvent.type;
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

		currentEvent.Init();
		initTimePassed = 0;
		OnEventChange?.Invoke(newEvent.type, EventStage.INIT);
	}

    public void StopEvent()
	{
		if (currentEvent != null)
		{
			OnEventChange?.Invoke(currentEventType, EventStage.END);
			currentEvent.Stop();
			currentEvent = null;
		}
	}

    public void UpdateEvent()
	{
		if (currentEvent != null)
		{
			if (initTimePassed < initTime)
			{
				initTimePassed += Time.deltaTime;
				if (initTimePassed >= initTime)
				{
					currentEvent.Start();
					OnEventChange?.Invoke(currentEventType, EventStage.BEGIN);
				}
			}
			else
			{
				currentEvent.Update();
			}
		}
	}
}
