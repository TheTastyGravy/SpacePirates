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
	public GameObject shipPrefab;
	public GameObject missilePrefab;
	public float enterTime;
	public float exitTime;
	public float firePeriod;
	public float spreadAngle;


	private GameObject playerShip;
	private GameObject shipObject;
	private Transform firepoint;

	private Vector3 followPos;
	private Vector3 offScreenPos;
	// Flag used to determine when to fire at the player
	private bool shipIsOnScreen = false;
	private float timePassed = 0;


	public override void Start()
	{
		playerShip = ShipManager.Instance.gameObject;
		Ship ship = Ship.GetShip(GameManager.SelectedShip);
		// Calculate the follow and off screen positions
		offScreenPos = -playerShip.transform.forward * ship.chaseShipOffScreenDist;
		offScreenPos += playerShip.transform.position;
		followPos = -playerShip.transform.forward * ship.chaseShipFollowDist;
		followPos += playerShip.transform.position;

		// Instantiate ship off screen and lerp it into position
		shipObject = Object.Instantiate(shipPrefab, offScreenPos, Quaternion.identity);
		EventManager.Instance.StartCoroutine(MoveShipOnScreen());

		// The firepoint is expected to be the first child
		firepoint = shipObject.transform.GetChild(0);
		spreadAngle *= Mathf.Deg2Rad * 0.5f;
	}

	public override void Stop()
	{
		// Lerp ship off screen, then destroy it
		EventManager.Instance.StartCoroutine(MoveShipOffScreen());
	}

	public override void Update()
	{
		if (!shipIsOnScreen)
		{
			return;
		}


		timePassed += Time.deltaTime;
		if (timePassed >= firePeriod)
		{
			timePassed = 0;

			// Get direction with random angle
			Vector3 dir = Vector3.zero;
			float angle = Random.Range(-spreadAngle, spreadAngle);
			dir.x = firepoint.forward.x * Mathf.Cos(angle) - firepoint.forward.z * Mathf.Sin(angle);
			dir.z = firepoint.forward.x * Mathf.Sin(angle) + firepoint.forward.z * Mathf.Cos(angle);
			// Instantiate missile with direction
			Object.Instantiate(missilePrefab, firepoint.position, Quaternion.FromToRotation(Vector3.forward, dir));
		}
	}


	private IEnumerator MoveShipOnScreen()
	{
		float time = 0;
		while (time < enterTime)
		{
			shipObject.transform.position = Vector3.Lerp(offScreenPos, followPos, time / enterTime);

			time += Time.deltaTime;
			yield return null;
		}

		shipIsOnScreen = true;
	}

	private IEnumerator MoveShipOffScreen()
	{
		float time = 0;
		while (time < exitTime)
		{
			shipObject.transform.position = Vector3.Lerp(followPos, offScreenPos, time / exitTime);

			time += Time.deltaTime;
			yield return null;
		}

		Object.Destroy(shipObject);
	}
}
