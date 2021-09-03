using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorStation : MonoBehaviour
{
    // Oxygen regen when turned on
    public float baseOxygenRegenRate = 5;
	[Tooltip("The ammount of time allowed between activating the switches")]
	public float timeBetweenSwitches = 0.25f;

	public Light stateLight;


	private ReactorSwitch[] reactorSwitches;
	private ReactorFuelGen fuelGen;
	private DamageStation damage;

    private bool isTurnedOn = true;
    public bool IsTurnedOn => isTurnedOn;
    private float currentOxygenRegen;
    public float CurrentOxygenRegen => currentOxygenRegen;

	// The time a switch was last used
	private float lastSwitchTime = 0;



    void Start()
    {
		reactorSwitches = GetComponentsInChildren<ReactorSwitch>();
		fuelGen = GetComponentInChildren<ReactorFuelGen>();
        damage = GetComponentInChildren<DamageStation>();

		// Add event listeners
		foreach (var reactorSwitch in reactorSwitches)
		{
			reactorSwitch.OnActivated += OnSwitchUsed;
		}
		damage.OnDamageTaken += OnDamageTaken;
		damage.OnDamageRepaired += OnDamageRepaired;

		currentOxygenRegen = baseOxygenRegenRate;
    }

	private void TurnOn()
	{
		isTurnedOn = true;
		currentOxygenRegen = baseOxygenRegenRate;
		fuelGen.isActive = true;

		stateLight.color = Color.green;
		stateLight.intensity = 0.5f;
	}

	private void TurnOff()
	{
		isTurnedOn = false;
		currentOxygenRegen = 0;
		fuelGen.isActive = false;

		stateLight.color = Color.red;
		stateLight.intensity = 2;
	}


	private void OnSwitchUsed()
	{
		if (lastSwitchTime + timeBetweenSwitches >= Time.time)
		{
			if (isTurnedOn)
				TurnOff();
			else
				TurnOn();
		}

		lastSwitchTime = Time.time;
	}

	private void OnDamageTaken()
	{
		TurnOff();

		foreach (var obj in reactorSwitches)
		{
			obj.enabled = false;
		}
	}

	private void OnDamageRepaired()
	{
		foreach (var obj in reactorSwitches)
		{
			obj.enabled = true;
		}
	}
}
