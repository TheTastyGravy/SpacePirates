using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineStation : MonoBehaviour
{
	public FuelIndicator fuelIndicator;
	public int maxFuel = 3;
	public float speedAcceleration = 1;
	public float speedDecay = 0.2f;
	[Space]
	public EngineParticleLogic engineEffect;

	private FuelDeposit fuelDepo;
	private DamageStation damage;
	public DamageStation Damage => damage;

	private float timePerFuel;
	private float maxSpeed;
	public float MaxSpeed => maxSpeed;
	private int startFuel;
	public bool IsTurnedOn => currentFuel > 0 && damage.DamageLevel == 0;
	private float currentSpeed = 0;
	public float CurrentSpeed => currentSpeed;
	private int currentFuel = 0;
	public int CurrentFuel => currentFuel;
	private float fuelTime = 0;

	

	void Awake()
	{
		enabled = false;
		fuelDepo = GetComponentInChildren<FuelDeposit>();
		damage = GetComponentInChildren<DamageStation>();
		fuelDepo.OnFuelDeposited += OnFueled;

		// Get values from difficulty settings
		LevelDificultyData.DiffSetting setting = GameManager.GetDifficultySettings();
		timePerFuel = setting.timePerFuel.Value;
		maxSpeed = setting.maxSpeed.Value;
		startFuel = setting.startFuel.Value;

		currentFuel = startFuel;
		if (currentFuel != 0)
			currentSpeed = maxSpeed;
		Invoke(nameof(FixFuelIndicator), 0.1f);
	}

	private void FixFuelIndicator()
	{
		fuelIndicator.SetFuelLevel((float)currentFuel / (float)maxFuel * 100f);
		if (currentFuel >= maxFuel)
			fuelDepo.enabled = false;
	}

	void Update()
	{
		if (IsTurnedOn)
		{
			engineEffect.Run();
			// Use fuel
			fuelTime += Time.deltaTime;
			if (fuelTime >= timePerFuel)
			{
				OnFuelUsed();
			}

			// Accelerate speed with cap
			if (currentSpeed < maxSpeed)
				currentSpeed += speedAcceleration * Time.deltaTime;
			else
				currentSpeed = maxSpeed;
		}
		else
		{
			engineEffect.Stop();
			// Decay speed with cap
			if (currentSpeed > 0)
				currentSpeed -= speedDecay * Time.deltaTime;
			else
				currentSpeed = 0;
		}

		fuelIndicator.SetFuelLevel(CalculateFuelValue() * 100f);
	}

	private void OnFueled()
	{
		currentFuel++;
		if (currentFuel >= maxFuel)
		{
			fuelDepo.enabled = false;
		}
	}

	private void OnFuelUsed()
	{
		currentFuel--;
		fuelTime = 0;
		fuelDepo.enabled = true;
	}

	private float CalculateFuelValue()
	{
		return (currentFuel - 1 + (1 - fuelTime / timePerFuel)) / (float)maxFuel;
	}
}
