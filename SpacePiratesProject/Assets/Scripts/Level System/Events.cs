using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event
{
	public abstract void Start();
	public abstract void Stop();
	public abstract void Update();
}


public class AstroidField : Event
{
	public float timeBetweenWaves;
	public int astroidsPerWave;

	private float timePassed = 0;



	public override void Start()
	{
		//activate background particle effects
	}

	public override void Stop()
	{
		//disable background particle effects
	}

	public override void Update()
	{
		timePassed += Time.deltaTime;
		if (timePassed >= timeBetweenWaves)
		{
			timePassed -= timeBetweenWaves;
			AstroidManager.Instance.SpawnAstroids(astroidsPerWave, true);
		}
	}
}

public class PlasmaStorm : Event
{
	public float timeBetweenDamage;

	private ShipManager shipManager;
	private float timePassed = 0;


	public override void Start()
	{
		shipManager = ShipManager.Instance;
		//start background effect
	}

	public override void Stop()
	{
		//stop background effect
	}

	public override void Update()
	{
		timePassed += Time.deltaTime;
		if (timePassed >= timeBetweenDamage)
		{
			timePassed -= timeBetweenDamage;

			DamageStation station = shipManager.GetRandomActiveStation();
			if (station != null)
			{
				station.Damage();
			}
			//play some effect at station (lightning? sparks?)
		}
	}
}

public class ShipAttack : Event
{
	public override void Start()
	{
	}

	public override void Stop()
	{
	}

	public override void Update()
	{
	}
}
