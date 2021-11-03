using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseShipLogic : MonoBehaviour
{
	public GameObject missilePrefab;
	public GameObject explosionPrefab;
	public Transform firePoint;
	public Light light1;
	public Light light2;
	[Tooltip("Used to place explosions when the ship is destroied")]
	public MeshFilter meshFilter;
	[Space]
	public float moveOnScreenTime = 1;
	public float moveOffScreenTime = 1;
	[Space]
	public float spreadAngle;
	[Space]
	public float wanderFrequency = 1;
	public float wanderDist = 0.75f;
	public float wanderSpeed = 3;
	public float wanderAcceleration = 1;
	[Space]
	public float timeBetweenExplosions = 0.5f;


	private Vector3 offScreenPos;
	private Vector3 followPos;
	private Coroutine moveRoutine = null;

	private int health;
	private float firePeriod;
	private float fireTimePassed = 0;
	private float wanderTimePassed = 0;
	private Vector3 wanderPos;
	private Vector3 velocity = Vector3.zero;
	// This should be determined using the particle effects on the prefab
	private float explosionTime = 1.5f;

	private float[] cumulativeSizes;
	private float total;



	void Start()
	{
		StartCoroutine(Siren());
	}

	public void Setup(int health, float firePeriod)
	{
		Transform playerShip = ShipManager.Instance.transform;
		Ship ship = Ship.GetShip(GameManager.SelectedShip);
		// Calculate the follow and off screen positions
		offScreenPos = -playerShip.forward * ship.chaseShipOffScreenDist;
		offScreenPos += playerShip.position;
		followPos = -playerShip.forward * ship.chaseShipFollowDist;
		followPos += playerShip.position;

		offScreenPos.y += 1.5f;
		followPos.y += 1.5f;

		this.health = health;
		this.firePeriod = firePeriod;
		spreadAngle *= Mathf.Deg2Rad * 0.5f;
		// Start moving on screen
		moveRoutine = StartCoroutine(Move(offScreenPos, followPos, moveOnScreenTime));
		// Get initial wander pos
		wanderPos = followPos + Random.insideUnitSphere * wanderDist;

		// Setup for GetRandomPointOnMesh()
		float[] sizes = GetTriSizes(meshFilter.mesh.triangles, meshFilter.mesh.vertices);
		cumulativeSizes = new float[sizes.Length];
		total = 0;
		for (int i = 0; i < sizes.Length; i++)
		{
			total += sizes[i];
			cumulativeSizes[i] = total;
		}
	}

    void Update()
    {
		// Do nothing while moving
		if (moveRoutine != null)
		{
			return;
		}


		// Fire missiles
		fireTimePassed += Time.deltaTime;
		if (fireTimePassed >= firePeriod)
		{
			fireTimePassed = 0;

			// Get direction with random angle
			Vector3 dir = Vector3.zero;
			float angle = Random.Range(-spreadAngle, spreadAngle);
			dir.x = firePoint.forward.x * Mathf.Cos(angle) - firePoint.forward.z * Mathf.Sin(angle);
			dir.z = firePoint.forward.x * Mathf.Sin(angle) + firePoint.forward.z * Mathf.Cos(angle);
			// Instantiate missile with direction
			Instantiate(missilePrefab, firePoint.position, Quaternion.FromToRotation(Vector3.forward, dir));
		}

		// Random wander
		wanderTimePassed += Time.deltaTime;
		if (wanderTimePassed >= wanderFrequency)
		{
			wanderTimePassed = 0;
			// Get new wander pos
			wanderPos = Random.insideUnitSphere * wanderDist;
			wanderPos.y = Mathf.Min(Mathf.Max(wanderPos.y, 0.5f), -0.5f);
			wanderPos += followPos;
		}
		// Adjust velocity with a sort of steering force, and apply to position
		velocity = Vector3.Lerp(velocity, (wanderPos - transform.position).normalized * wanderSpeed, Time.deltaTime * wanderAcceleration);
		transform.position += velocity * Time.deltaTime;
	}

	void OnCollisionEnter(Collision collision)
	{
		// If we are hit by a turret
		if (collision.gameObject.CompareTag("TurretProjectile"))
		{
			// Destroy the turret projectile. this might allow multiple collision events to fire
			Destroy(collision.gameObject);
			// Create explosion effect, and destroy it after its ended
			Destroy(Instantiate(explosionPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal, Random.onUnitSphere)), explosionTime);

			health--;
			if (health <= 0)
			{
				EventManager.Instance.StopEvent();
			}
		}
	}


	// Called when the event ends
	public void OnEventEnd()
	{
		// If we are (presumably) still moving forward, cancel it
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
		}

		// Move off screen, then destroy the ship
		moveRoutine = StartCoroutine(Move(transform.position, offScreenPos, moveOffScreenTime));
		Destroy(gameObject, moveOffScreenTime);

		// If the event ended because we ran out of health, start exploding
		if (health <= 0)
		{
			StartCoroutine(Explode());
		}
	}

    private IEnumerator Move(Vector3 start, Vector3 end, float time)
	{
        float timePassed = 0;
        while (timePassed < time)
		{
            transform.position = Vector3.Lerp(start, end, timePassed / time);

            timePassed += Time.deltaTime;
            yield return null;
		}

		moveRoutine = null;
	}

	private IEnumerator Siren()
	{
		while (true)
		{
			light1.enabled = true;
			light2.enabled = false;
			yield return new WaitForSeconds(0.3f);
			light1.enabled = false;
			light2.enabled = true;
			yield return new WaitForSeconds(0.3f);
		}
	}

	private IEnumerator Explode()
	{
		while (true)
		{
			// Meshes are concidered in local space, so it needs to be converted
			Vector3 point = meshFilter.transform.localToWorldMatrix * GetRandomPointOnMesh();
			// Create explosion effect, and destroy it after its done
			GameObject explosionInstance = Instantiate(explosionPrefab, point + transform.position, Quaternion.LookRotation(point - meshFilter.mesh.bounds.center, Random.onUnitSphere), transform);
			Destroy(explosionInstance, explosionTime);

			yield return new WaitForSeconds(timeBetweenExplosions);
		}
	}

	// Get a random position on the surface of a mesh
	private Vector3 GetRandomPointOnMesh()
	{
		Mesh mesh = meshFilter.mesh;

		float randomsample = Random.value * total;
		int triIndex = -1;
		for (int i = 0; i < cumulativeSizes.Length; i++)
		{
			if (randomsample <= cumulativeSizes[i])
			{
				triIndex = i;
				break;
			}
		}

		if (triIndex == -1) Debug.LogError("triIndex should never be -1");

		Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
		Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
		Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

		//generate random barycentric coordinates

		float r = Random.value;
		float s = Random.value;
		if (r + s >= 1)
		{
			r = 1 - r;
			s = 1 - s;
		}
		//and then turn them back to a Vector3
		Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
		return pointOnMesh;
	}

	private float[] GetTriSizes(int[] tris, Vector3[] verts)
	{
		int triCount = tris.Length / 3;
		float[] sizes = new float[triCount];
		for (int i = 0; i < triCount; i++)
		{
			sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
		}
		return sizes;
	}
}
