using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidManager : Singleton<AstroidManager>
{
	public GameObject astroidPrefab;
	public float timeBetweenAstroids = 5;
	public float maxAngleVariance = 45;
	public float astroidSpawnDelay = 1.5f;
	public LayerMask raycastMask;
	public float raycastYOffset = 1;
	[Space]
	public float uiEdgeRadius = 25;
	public GameObject uiPrefab;
	public Transform canvas;
	public PolygonCollider2D uiBoundry;
	public float uiExtraTime = 0.5f;


	private BoxCollider[] regions;
	private float timePassed = 0;
	private Vector2 screenScale;



	void Start()
	{
		maxAngleVariance *= 0.5f;
		screenScale = Screen.safeArea.size * new Vector2(1f / 1920f, 1f / 1080f);

		ShrinkBoundry();
		Invoke("SetupRegions", 0.1f);
	}

	private void ShrinkBoundry()
	{
		int Loop(int i, int size)
		{
			if (i < 0)
				i += size;

			return i % size;
		}

		// Shrink the boundry by the edge radius
		// Note that this is not a full solution, and only works in cases with corners
		Vector2 posScale = screenScale * uiEdgeRadius;
		Vector2[] points = uiBoundry.points;
		for (int i = 0; i < points.Length; i++)
		{
			points[i] *= screenScale;
			Vector2 dir1 = (uiBoundry.points[Loop(i - 1, points.Length)] * screenScale) - points[i];
			Vector2 dir2 = (uiBoundry.points[Loop(i + 1, points.Length)] * screenScale) - points[i];
			Vector2 diff = (dir1.normalized + dir2.normalized) * posScale;

			// Either add or subtract depending if concave or convex
			if (Vector2.SignedAngle(dir1, dir2) > 0)
				points[i] -= diff;
			else
				points[i] += diff;
		}
		uiBoundry.points = points;
	}

	private void SetupRegions()
	{
		List<BoxCollider> boxes = new List<BoxCollider>();
		Camera cam = Camera.main;
		Plane plane = new Plane(Vector3.up, 0);
		float boxDepth = 5;
		float extraDist = 2;

		float ySize = cam.orthographicSize;
		float xSize = ySize * cam.aspect;
		float extraSize = 3;

		// Determine the Y rotation for each box
		Vector3 camForward = cam.transform.forward;
		camForward.y = 0;
		float rotation = Vector3.SignedAngle(Vector3.forward, camForward, Vector3.up) + 90;


		void CreateBox(string name, Vector2 viewCoord, Vector3 boxSize)
		{
			// Get position from viewport on plane
			Ray ray = cam.ViewportPointToRay(viewCoord);
			plane.Raycast(ray, out float enter);
			Vector3 position = ray.GetPoint(enter);
			// Add offset in direction relitive to camera
			Vector3 dir = cam.transform.TransformPoint(viewCoord - new Vector2(.5f, .5f)) - cam.transform.position;
			dir.y = 0;
			position += dir.normalized * (boxDepth * 0.5f + extraDist);

			// Create object with box collider
			GameObject obj = new GameObject(name);
			obj.transform.parent = transform;
			obj.transform.position = position;
			obj.transform.eulerAngles = new Vector3(0, rotation, 0);
			BoxCollider box = obj.AddComponent<BoxCollider>();
			box.size = boxSize;
			box.isTrigger = true;
			boxes.Add(box);
		}

		CreateBox("Top", new Vector2(.5f, 1), new Vector3(boxDepth, 1, xSize * 2 + extraSize));
		CreateBox("Left", new Vector2(0, .5f), new Vector3(ySize * 2 + extraSize, 1, boxDepth));
		CreateBox("Right", new Vector2(1, .5f), new Vector3(ySize * 2 + extraSize, 1, boxDepth));
		regions = boxes.ToArray();
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
			Vector3 rayOrigin = pos;
			rayOrigin.y += raycastYOffset;
			bool hasHit;
			int iter = 0;
			do
			{
				float lookAngle = Random.Range(-maxAngleVariance, maxAngleVariance) * Mathf.Deg2Rad;
				// Get angle as a direction
				dir.x = baseDirection.x * Mathf.Cos(lookAngle) - baseDirection.z * Mathf.Sin(lookAngle);
				dir.z = baseDirection.x * Mathf.Sin(lookAngle) + baseDirection.z * Mathf.Cos(lookAngle);
				hasHit = Physics.Raycast(rayOrigin, dir, 100, raycastMask, QueryTriggerInteraction.Ignore);
				++iter;
			} while (hasHit != willHit && iter <= 10);

			// If we should hit but missed, target (0, 0)
			if (willHit && !hasHit)
			{
				dir = -pos;
				dir.y = 0;
				dir.Normalize();
			}

			// Get astroid info in screen space to generate a raycast
			Vector2 screenPoint = Camera.main.WorldToScreenPoint(pos);
			Vector2 screenDir = (Vector2)Camera.main.WorldToScreenPoint(pos + dir) - screenPoint;
			RaycastHit2D rayHit = Physics2D.Raycast(screenPoint, screenDir);
			// Create UI element
			GameObject uiObj = Instantiate(uiPrefab, canvas);
			Destroy(uiObj, astroidSpawnDelay + uiExtraTime);
			RectTransform rectTrans = uiObj.transform as RectTransform;
			rectTrans.localScale *= screenScale;
			// Set its position on the canvas
			rectTrans.position = Camera.main.ScreenToWorldPoint(rayHit.point);
			rectTrans.localPosition = new Vector3(rectTrans.localPosition.x, rectTrans.localPosition.y, 0);
			// Orientate the object with the canvas
			rectTrans.rotation = Quaternion.LookRotation(canvas.forward, Vector3.Cross(canvas.forward, canvas.transform.TransformDirection(screenDir)));

			// Create astroid
			AstroidLogic astroid = Instantiate(astroidPrefab).GetComponent<AstroidLogic>();
			astroid.Setup(pos, dir, astroidSpawnDelay);
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
