using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidManager : Singleton<AstroidManager>
{
	public GameObject astroidPrefab;
	public float timeBetweenAstroids = 5;
	public float maxAngleVariance = 45;
	public LayerMask raycastMask;

	private BoxCollider[] regions;
	private float timePassed = 0;



	void Start()
	{
		regions = GetComponentsInChildren<BoxCollider>();
		maxAngleVariance *= 0.5f;
	}

	void Update()
	{
		timePassed += Time.deltaTime;
		if (timePassed >= timeBetweenAstroids)
		{
			timePassed = 0;
			SpawnAstroids(1);
		}
	}


	public void SpawnAstroids(int count, bool canMiss = false)
	{
		BoxCollider region = regions[Random.Range(0, regions.Length)];
		Vector3 baseDirection = (Vector3.zero - region.transform.position).normalized;

		for (int i = 0; i < count; i++)
		{
			// Get position in region
			Vector3 pos = GetRandomPointInsideCollider(region);

			bool willHit = true;
			if (canMiss)
			{
				willHit = Random.Range(0f, 1f) > ShipManager.Instance.GetShipAvoidance();
			}
			// Get a random direction until we find one that either hits or misses the ship
			Vector3 dir = Vector3.zero;
			int iter = 0;
			do
			{
				float lookAngle = Random.Range(-maxAngleVariance, maxAngleVariance) * Mathf.Deg2Rad;
				// Get angle as a direction
				dir.x = baseDirection.x * Mathf.Cos(lookAngle) - baseDirection.z * Mathf.Sin(lookAngle);
				dir.z = baseDirection.x * Mathf.Sin(lookAngle) + baseDirection.z * Mathf.Cos(lookAngle);
				iter++;
			} while (Physics.Raycast(pos, dir, 100, raycastMask, QueryTriggerInteraction.Ignore) != willHit || iter < 10);


			// Create astroid
			AstroidLogic astroid = Instantiate(astroidPrefab).GetComponent<AstroidLogic>();
			astroid.Setup(pos, dir);
		}
	}

	private Vector3 GetRandomPointInsideCollider(BoxCollider boxCollider)
	{
		Vector3 extents = boxCollider.size * 0.5f;
		Vector3 point = new Vector3(
			Random.Range(-extents.x, extents.x),
			Random.Range(-extents.y, extents.y),
			Random.Range(-extents.z, extents.z)
		);

		point = boxCollider.transform.rotation * point;
		point += boxCollider.transform.position;
		return point;
	}
}
