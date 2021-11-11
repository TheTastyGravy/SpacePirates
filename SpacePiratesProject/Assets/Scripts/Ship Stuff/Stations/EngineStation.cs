using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EngineStation : MonoBehaviour
{
	public FuelIndicator fuelIndicator;
	public float speedAcceleration = 1;
	public float speedDecay = 0.2f;
	[Space]
	public EngineParticleLogic engineEffect;
	public EventReference ignitionEvent;

	private FuelDeposit fuelDepo;
	private DamageStation damage;
	public DamageStation Damage => damage;

	public bool IsTurnedOn => currentFuel > 0 && damage.DamageLevel == 0;
	private float maxSpeed;
	public float MaxSpeed => maxSpeed;
	private float currentSpeed = 0;
	public float CurrentSpeed => currentSpeed;


	private float maxFuel;
	private float ammountOnRefuel;
	private float fuelUsageRate;
	private float startFuel;

	private float currentFuel = 0;
	public float CurrentFuel => currentFuel;

	

	void Awake()
	{
		enabled = false;
		fuelDepo = GetComponentInChildren<FuelDeposit>();
		damage = GetComponentInChildren<DamageStation>();
		fuelDepo.OnFuelDeposited += OnFueled;
		damage.OnDamageRepaired += OnDamageRepaired;

		// Get values from difficulty settings
		LevelDificultyData.DiffSetting setting = GameManager.GetDifficultySettings();
		maxFuel = setting.maxFuel.Value;
		startFuel = setting.startFuel.Value;
		ammountOnRefuel = setting.ammountOnRefuel.Value;
		fuelUsageRate = setting.fuelUsageRate.Value;
		maxSpeed = setting.maxSpeed.Value;

		currentFuel = startFuel;
		if (currentFuel > 0)
			currentSpeed = maxSpeed;
		Invoke(nameof(FixFuelIndicator), 0.1f);
	}

	private void FixFuelIndicator()
	{
		fuelIndicator.SetFuelLevel(currentFuel / maxFuel * 100f);
		if (currentFuel >= maxFuel)
			fuelDepo.enabled = false;
	}

	void Update()
	{
		if (IsTurnedOn)
		{
			engineEffect.Run();
			// Use fuel
			currentFuel -= Time.deltaTime * fuelUsageRate;
			if (currentFuel <= 0)
			{
				currentFuel = 0;
				return;
			}
			else if (currentFuel < maxFuel)
			{
				fuelDepo.enabled = true;
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

		fuelIndicator.SetFuelLevel(currentFuel / maxFuel * 100f);
	}

	private void OnFueled()
	{
		currentFuel += ammountOnRefuel;
		if (currentFuel >= maxFuel)
		{
			currentFuel = maxFuel;
			fuelDepo.enabled = false;
		}

		if (currentFuel > 0 && damage.DamageLevel == 0)
		{
			RuntimeManager.PlayOneShot(ignitionEvent);
		}
	}

	private void OnDamageRepaired()
	{
		if (currentFuel > 0)
		{
			RuntimeManager.PlayOneShot(ignitionEvent);
		}
	}
}
