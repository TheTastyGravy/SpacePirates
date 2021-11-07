using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorStation : MonoBehaviour
{
	public Light stateLight;
	public ParticleSystem ActiveEffect;

	private BasicSwitch[] reactorSwitches;
	private ReactorFuelGen fuelGen;
	private DamageStation damage;
	public DamageStation Damage => damage;
	private bool isTurnedOn = true;
    public bool IsTurnedOn => isTurnedOn;
    private float currentOxygenRegen;
    public float CurrentOxygenRegen => currentOxygenRegen;

	private float baseOxygenRegenRate = 5;
	private float timeBetweenSwitches = 0.25f;
	// The time a switch was last used
	private float lastSwitchTime = 0;



    void Awake()
    {
		reactorSwitches = GetComponentsInChildren<BasicSwitch>();
		fuelGen = GetComponentInChildren<ReactorFuelGen>();
        damage = GetComponentInChildren<DamageStation>();
		enabled = false;

		// Get values from difficulty settings
		LevelDificultyData.DiffSetting setting = GameManager.GetDifficultySettings();
		baseOxygenRegenRate = setting.baseOxygenRegenRate.Value;
		timeBetweenSwitches = setting.timeBetweenSwitches.Value;

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
		fuelGen.SetActive(true);

		stateLight.color = Color.green;
		stateLight.intensity = 1;
		ActiveEffect.Play();
	}

	private void TurnOff()
	{
		isTurnedOn = false;
		currentOxygenRegen = 0;
		fuelGen.SetActive(false);

		stateLight.color = Color.red;
		stateLight.intensity = 2;
		ActiveEffect.Stop();
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
			obj.forceDisabled = true;
		}
	}

	private void OnDamageRepaired()
	{
		foreach (var obj in reactorSwitches)
		{
			obj.enabled = true;
			obj.forceDisabled = false;
		}
	}

	void OnEnable()
	{
		fuelGen.isActive = isTurnedOn;
	}

	void OnDisable()
	{
		if (fuelGen)
			fuelGen.isActive = false;
	}
}
