using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineStation : MonoBehaviour
{
	public FuelIndicator fuelIndicator;
	public int maxFuel = 3;
	public int startFuel = 0;
	[Tooltip("How long each fuel lasts")]
	public float timePerFuel = 10;
	[Tooltip("The speed allied to the ship when turned on")]
	public float maxSpeed = 1;
	public float speedAcceleration = 1;
	public float speedDecay = 0.2f;

	private EngineSwitch engineSwitch;
	private FuelDeposit fuelDepo;
	private DamageStation damage;
	public DamageStation Damage => damage;

	private bool isTurnedOn = false;
	public bool IsTurnedOn => isTurnedOn;
	private float currentSpeed = 0;
	public float CurrentSpeed => currentSpeed;

	private int currentFuel = 0;
	public int CurrentFuel => currentFuel;
	private float fuelTime = 0;



	void Awake()
	{
		engineSwitch = GetComponentInChildren<EngineSwitch>();
		fuelDepo = GetComponentInChildren<FuelDeposit>();
		damage = GetComponentInChildren<DamageStation>();

		engineSwitch.engine = this;
		engineSwitch.OnActivated += OnSwitchUsed;
		fuelDepo.OnFuelDeposited += OnFueled;

		currentFuel = startFuel;
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
		if (isTurnedOn)
		{
			// If we have taken damage, turn off
			if (damage.DamageLevel > 0)
			{
				isTurnedOn = false;
				return;
			}

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
			// We can only be turned on if we have fuel and arnt damaged
			engineSwitch.enabled = currentFuel > 0 && damage.DamageLevel == 0;

			// Decay speed with cap
			if (currentSpeed > 0)
				currentSpeed -= speedDecay * Time.deltaTime;
			else
				currentSpeed = 0;
		}
	}

	private void OnSwitchUsed()
	{
		isTurnedOn = !isTurnedOn;
	}

	private void OnFueled()
	{
		currentFuel++;
		if (currentFuel >= maxFuel)
		{
			fuelDepo.enabled = false;
		}

		fuelIndicator.SetFuelLevel((float)currentFuel / (float)maxFuel * 100f);
	}

	private void OnFuelUsed()
	{
		currentFuel--;
		fuelTime = 0;
		fuelDepo.enabled = true;

		if (currentFuel == 0)
		{
			isTurnedOn = false;
		}

		fuelIndicator.SetFuelLevel((float)currentFuel / (float)maxFuel * 100f);
	}
}
