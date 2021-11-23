using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event
{
	public virtual void Init() { }
	public abstract void Start();
	public abstract void Stop();
	public abstract void Update();
}


public class AstroidField : Event
{
	public float preStrikeDelay;
	public float timeBetweenWaves;
	public int astroidsPerWave;

	private float timePassed = 0;
	private bool donePreStrike = false;


	public override void Start()
	{
	}

	public override void Stop()
	{
	}

	public override void Update()
	{
		timePassed += Time.deltaTime;
		if (timePassed >= timeBetweenWaves - preStrikeDelay && !donePreStrike)
		{
			donePreStrike = true;
			PreStrike();
		}
		else if (timePassed >= timeBetweenWaves)
		{
			timePassed -= timeBetweenWaves;
			donePreStrike = false;
			Strike();
		}
	}

	private void PreStrike()
	{
		StatusManager.Instance.OnPrestrike(Level.Event.Type.AstroidField);
	}

	private void Strike()
	{
		AstroidManager.Instance.SpawnAstroids(astroidsPerWave, true);
	}
}

public class PlasmaStorm : Event
{
	public RectTransform canvas;
	public GameObject strikeEffect;
	public float preStrikeDelay;
	public float timeBetweenDamage;

	private ShipManager shipManager;
	private float timePassed = 0;
	private bool donePreStrike = false;


	public override void Init()
	{
		shipManager = ShipManager.Instance;
	}

	public override void Start()
	{
	}

	public override void Stop()
	{
	}

	public override void Update()
	{
		timePassed += Time.deltaTime;

		if (timePassed >= timeBetweenDamage - preStrikeDelay && !donePreStrike)
		{
			donePreStrike = true;
			PreStrike();
		}
		else if (timePassed >= timeBetweenDamage)
		{
			timePassed -= timeBetweenDamage;
			donePreStrike = false;
			Strike();
		}
	}

	private void PreStrike()
	{
		StatusManager.Instance.OnPrestrike(Level.Event.Type.PlasmaStorm);
	}

	private void Strike()
	{
		DamageStation station = shipManager.GetRandomActiveStation();
		if (station != null)
		{
			station.Damage();
            Object.FindObjectOfType<LightningStrikeEffect>().CreateStrike(station.transform.position, 2);
        }

		Object.Instantiate(strikeEffect, canvas);
		CameraManager.Instance.Shake();
	}
}

public class ShipAttack : Event
{
	public GameObject shipPrefab;
	public float zoomLevel;
	public int shipHealth;
	public float firePeriod;
	public float initTime;
	
	private ChaseShipLogic shipLogic;
	private CameraManager camManager;


	public override void Init()
	{
		camManager = CameraManager.Instance;
		// Setup ship
		shipLogic = Object.Instantiate(shipPrefab).GetComponent<ChaseShipLogic>();
		shipLogic.moveOnScreenTime = initTime;
		shipLogic.Setup(shipHealth, firePeriod);
	}

	public override void Start()
	{
		shipLogic.initOver = true;
		camManager.Zoom(zoomLevel);
	}

	public override void Stop()
	{
		//tell ship to move off screen
		shipLogic.OnEventEnd();
		camManager.Zoom(1);
	}

	public override void Update()
	{
	}
}
