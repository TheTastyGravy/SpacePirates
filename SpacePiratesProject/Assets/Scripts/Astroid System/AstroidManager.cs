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


	public float uiEdgeRadius = 25;
	public GameObject uiPrefab;
	public Transform canvas;
	public PolygonCollider2D uiBoundry;



	void Start()
	{
		regions = GetComponentsInChildren<BoxCollider>();
		maxAngleVariance *= 0.5f;

		// Shrink the boundry by the edge radius
		Vector2[] points = uiBoundry.points;
		for (int i = 0; i < points.Length; i++)
		{
			Vector2 dir1 = uiBoundry.points[Loop(i - 1, points.Length)] - points[i];
			Vector2 dir2 = uiBoundry.points[Loop(i + 1, points.Length)] - points[i];
			Vector2 diff = (dir1.normalized + dir2.normalized) * uiEdgeRadius;

			// Either add or subtract depending if concave or convex
			if (Vector2.SignedAngle(dir1, dir2) > 0)
				points[i] -= diff;
			else
				points[i] += diff;
		}
		uiBoundry.points = points;
	}

	int Loop(int i, int size)
	{
		if (i < 0)
			i += size;
		
		return i % size;
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


			// Get astroid info in screen space to generate a raycast
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(pos);
			Vector2 screenDir = (Vector2)Camera.main.WorldToScreenPoint(pos + dir) - screenPoint;
			RaycastHit2D rayHit = Physics2D.Raycast(screenPoint, screenDir);
			// Create UI element
			GameObject uiObj = Instantiate(uiPrefab, canvas);
			(uiObj.transform as RectTransform).position = rayHit.point;
			(uiObj.transform as RectTransform).right = screenDir;
			Destroy(uiObj, 3);

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
