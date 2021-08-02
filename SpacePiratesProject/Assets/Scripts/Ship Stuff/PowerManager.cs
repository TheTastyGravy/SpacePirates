using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : Singleton<PowerManager>
{
	public int maxPower = 4;
	public float tickInterval = 1;
	
	[HideInInspector]
	public int tempEnergyUsage = 0;
	private float timePassed = 0;
	private EngineStation[] engines;
	private DamageStation[] reactors;



	void Awake()
	{
		engines = GetComponentsInChildren<EngineStation>();

		// Get damage stations with reactor tag
		List<DamageStation> damageStations = new List<DamageStation>();
		foreach (var obj in GetComponentsInChildren<DamageStation>())
		{
			if (obj.CompareTag("Reactor"))
				damageStations.Add(obj);
		}
		reactors = damageStations.ToArray();
	}

	void Update()
    {
		// Wait for time interval
		timePassed += Time.deltaTime;
		while (timePassed >= tickInterval)
		{
			timePassed -= tickInterval;

			// Get total energy usage
			int powerUsage = tempEnergyUsage;
			foreach (var obj in engines)
			{
				powerUsage += obj.PowerLevel;
			}

			// If energy usage is over the max, roll a chance to take damage
			if (powerUsage > maxPower)
			{
				// Determine the chance of taking damage
				var chance = (powerUsage - maxPower) switch
				{
					1 => 0.1f,
					2 => 0.25f,
					3 => 0.5f,
					4 => 0.75f,
					_ => 1f,
				};
				if (Random.Range(0f, 1f) < chance)
				{
					// Damage a random reactor
					int index = Random.Range(0, reactors.Length);
					reactors[index].Damage();
				}
			}

			// Reset temp energy
			tempEnergyUsage = 0;
		}


		// Check if a reactor has been destroied
		foreach (var obj in reactors)
		{
			if (obj.DamageLevel == obj.maxDamageLevel)
			{
				//game over
			}
			else if (obj.DamageLevel == obj.maxDamageLevel - 1)
			{
				SoundController.Instance.Play("ReactorAlert", false);
			}
		}
	}
}
