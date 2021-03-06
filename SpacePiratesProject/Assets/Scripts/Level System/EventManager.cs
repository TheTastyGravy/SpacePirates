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
	private float astroidsPerWave;
	private float asteroidPrestrikeDelay;

	public RectTransform canvas;
	public GameObject strikeEffect;
	private float timeBetweenDamage;
	private float plasmaStormPretrikeDelay;

	public GameObject shipPrefab;
	public float shipZoomLevel;
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
		asteroidPrestrikeDelay = settings.asteroidPrestrikeDelay;
		timeBetweenDamage = settings.timeBetweenStormDamage;
		plasmaStormPretrikeDelay = settings.plasmaStormPretrikeDelay;
		shipHealth = settings.shipHealth;
		firePeriod = settings.shipFirePeriod;
	}

    public void UpdateValues()
    {
        // Apply endless mode value modifications
        LevelDificultyData diffData = GameManager.DiffData;
        initTime -= diffData.initTimeDecrease;
        timeBetweenWaves -= diffData.timeBetweenAsteroidWavesDecrease;
        astroidsPerWave += diffData.asteroidsPerWaveIncrease;
        asteroidPrestrikeDelay -= diffData.asteroidPrestrikeDelayDecrease;
        timeBetweenDamage -= diffData.timeBetweenStormDamageDecrease;
        plasmaStormPretrikeDelay -= diffData.plasmaStormPretrikeDelayDecrease;
        shipHealth += diffData.shipHealthIncrease;
        firePeriod += diffData.shipFirePeriodIncrease;
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
					astroidsPerWave = Mathf.FloorToInt(astroidsPerWave),
					preStrikeDelay = asteroidPrestrikeDelay
				};
				break;
			case Level.Event.Type.PlasmaStorm:
				currentEvent = new PlasmaStorm()
				{
					canvas = canvas,
					strikeEffect = strikeEffect,
					timeBetweenDamage = timeBetweenDamage,
					preStrikeDelay = plasmaStormPretrikeDelay
				};
				break;
			case Level.Event.Type.ShipAttack:
				currentEvent = new ShipAttack()
				{
					shipPrefab = shipPrefab,
					zoomLevel = shipZoomLevel,
					shipHealth = shipHealth,
					firePeriod = firePeriod,
					initTime = initTime
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
