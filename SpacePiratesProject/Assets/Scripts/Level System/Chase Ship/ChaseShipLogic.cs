using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChaseShipLogic : MonoBehaviour
{
	public Transform firePoint;
	public Transform turret;
	public Transform turretRotationPoint;
	public Light light1;
	public Light light2;
	[Tooltip("Used to place explosions when the ship is destroied")]
	public MeshFilter meshFilter;
	[Space]
	public float moveOnScreenTime = 1;
	public float moveOffScreenTime = 1;
	[Space]
	public float spreadAngle;
	public float rotationSpeed = 5;
	[Space]
	public float wanderFrequency = 1;
	public float wanderSpeed = 3;
	public float wanderAcceleration = 1;
	[Space]
	public float timeBetweenExplosions = 0.5f;

	[HideInInspector]
	public bool initOver = false;
	private Transform trans;
    private ObjectPool explosionPool;
    private ObjectPool missilePool;
	private Ship ship;
	private Vector3 wanderAreaCenter;
	private Vector3 wanderAreaSize;
	private float offScrenDist;
	private Coroutine moveRoutine = null;
	private int health;
	private float firePeriod;
	private float fireTimePassed = 0;
	private float currentAngle = 0;
	private float targetAngle = 0;
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
		trans = transform;
        explosionPool = ObjectPool.GetPool("Explosion Pool");
        missilePool = ObjectPool.GetPool("Missile Pool");
        ship = Ship.GetShip(GameManager.SelectedShip);

		wanderAreaCenter = ship.chaseShipWanderCenterOffset;
		wanderAreaSize = ship.chaseShipWanderArea;
		offScrenDist = ship.chaseShipOffScreenDist;

		this.health = health;
		this.firePeriod = firePeriod;
		// Start moving on screen
		moveRoutine = StartCoroutine(Move(wanderAreaCenter + Vector3.back * offScrenDist, wanderAreaCenter, moveOnScreenTime));
		// Get initial wander pos
		wanderPos = Random.insideUnitSphere;
		wanderPos.Scale(wanderAreaSize);
		wanderPos += wanderAreaCenter;

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
		if (moveRoutine != null || !initOver)
			return;

		UpdatePosition();
		UpdateTurret();
	}

	private void UpdatePosition()
	{
		//TODO: include move to remove jank

		// Update values from ship for debugging
		wanderAreaCenter = ship.chaseShipWanderCenterOffset;
		wanderAreaSize = ship.chaseShipWanderArea;

		// Random wander
		wanderTimePassed += Time.deltaTime;
		if (wanderTimePassed >= wanderFrequency)
		{
			wanderTimePassed = 0;
			// Get new wander pos
			wanderPos = Random.insideUnitSphere;
			wanderPos.Scale(wanderAreaSize);
			wanderPos += wanderAreaCenter;
		}

		// Adjust velocity with a sort of steering force, and apply to position
		velocity = Vector3.Lerp(velocity, (wanderPos - trans.position).normalized * wanderSpeed, Time.deltaTime * wanderAcceleration);
		trans.position += velocity * Time.deltaTime;
	}

	private void UpdateTurret()
	{
		if (Mathf.Abs(Mathf.Abs(currentAngle) - Mathf.Abs(targetAngle)) < 1)
		{
			targetAngle = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
		}
		float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
		turret.RotateAround(turretRotationPoint.position, Vector3.up, currentAngle - newAngle);
		currentAngle = newAngle;

		// Fire missiles
		fireTimePassed += Time.deltaTime;
		if (fireTimePassed >= firePeriod)
		{
			fireTimePassed = 0;
            missilePool.GetInstance().transform.SetPositionAndRotation(firePoint.position, Quaternion.FromToRotation(Vector3.forward, firePoint.forward));
		}
	}

	void OnCollisionEnter(Collision collision)
	{
        void Damage()
        {
            // Create explosion effect
            GameObject explosion = explosionPool.GetInstance();
            explosion.transform.SetPositionAndRotation(collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal, Random.onUnitSphere));
            explosionPool.Return(explosion, explosionTime);

            health--;
            if (health <= 0)
            {
                EventManager.Instance.StopEvent();
            }
        }
        
        // If we are hit by a turret or asteroid
        if (collision.gameObject.CompareTag("TurretProjectile"))
		{
            // Destroy the projectile
            ObjectPool.GetPool("Projectile Pool").Return(collision.gameObject);
            Damage();
        }
        else if (LayerMask.LayerToName(collision.gameObject.layer) == "Astroid")
        {
            // Destroy the asteroid
            ObjectPool.GetPool("Asteroid Pool").Return(collision.gameObject);
            Damage();
        }
	}

	// Called when the event ends
	public void OnEventEnd()
	{
		if (moveRoutine != null)
			StopCoroutine(moveRoutine);
		moveRoutine = StartCoroutine(Move(trans.position, trans.position + Vector3.back * offScrenDist, moveOffScreenTime));
		Destroy(gameObject, moveOffScreenTime);

		// If the event ended because we ran out of health, start exploding
		if (health <= 0)
		{
			StartCoroutine(Explode());
		}
	}

    private IEnumerator Move(Vector3 start, Vector3 end, float time)
	{
		//move to update
        float timePassed = 0;
        while (timePassed < time)
		{
			trans.position = Vector3.Lerp(start, end, timePassed / time);

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
		Transform meshTrans = meshFilter.transform;
		while (true)
		{
			// Meshes are concidered in local space, so it needs to be converted
			Vector3 point = meshTrans.localToWorldMatrix * GetRandomPointOnMesh();
            // Create explosion effect, and destroy it after its done
            GameObject explosion = explosionPool.GetInstance();
            explosion.transform.SetPositionAndRotation(point + trans.position, Quaternion.LookRotation(point - meshFilter.mesh.bounds.center, Random.onUnitSphere));
            explosionPool.Return(explosion, explosionTime);

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


#if UNITY_EDITOR
	[DrawGizmo(GizmoType.Pickable)]
	private void OnDrawGizmos()
	{
		Vector3 drawPos = wanderAreaCenter;
		Vector3 elipseSize = wanderAreaSize;
		float size = 1;
		Color col = Color.white;

		//Y-Z Ring
		Handles.DrawBezier(new Vector3(drawPos.x, drawPos.y + elipseSize.y, drawPos.z), new Vector3(drawPos.x, drawPos.y, drawPos.z + elipseSize.z), new Vector3(0, elipseSize.y, elipseSize.z / 2)   + drawPos, new Vector3(0, elipseSize.y / 2, elipseSize.z)   + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x, drawPos.y + elipseSize.y, drawPos.z), new Vector3(drawPos.x, drawPos.y, drawPos.z - elipseSize.z), new Vector3(0, elipseSize.y, -elipseSize.z / 2)  + drawPos, new Vector3(0, elipseSize.y / 2, -elipseSize.z)  + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x, drawPos.y - elipseSize.y, drawPos.z), new Vector3(drawPos.x, drawPos.y, drawPos.z + elipseSize.z), new Vector3(0, -elipseSize.y, elipseSize.z / 2)  + drawPos, new Vector3(0, -elipseSize.y / 2, elipseSize.z)  + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x, drawPos.y - elipseSize.y, drawPos.z), new Vector3(drawPos.x, drawPos.y, drawPos.z - elipseSize.z), new Vector3(0, -elipseSize.y, -elipseSize.z / 2) + drawPos, new Vector3(0, -elipseSize.y / 2, -elipseSize.z) + drawPos, col, Texture2D.whiteTexture, size);
		//X-Y Ring
		Handles.DrawBezier(new Vector3(drawPos.x + elipseSize.x, drawPos.y, drawPos.z), new Vector3(drawPos.x, drawPos.y + elipseSize.y, drawPos.z), new Vector3((elipseSize.x), (elipseSize.y / 2), 0)   + drawPos, new Vector3((elipseSize.x / 2), (elipseSize.y), 0)   + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x - elipseSize.x, drawPos.y, drawPos.z), new Vector3(drawPos.x, drawPos.y + elipseSize.y, drawPos.z), new Vector3(-(elipseSize.x), (elipseSize.y / 2), 0)  + drawPos, new Vector3(-(elipseSize.x / 2), (elipseSize.y), 0)  + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x + elipseSize.x, drawPos.y, drawPos.z), new Vector3(drawPos.x, drawPos.y - elipseSize.y, drawPos.z), new Vector3((elipseSize.x), -(elipseSize.y / 2), 0)  + drawPos, new Vector3((elipseSize.x / 2), -(elipseSize.y), 0)  + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x - elipseSize.x, drawPos.y, drawPos.z), new Vector3(drawPos.x, drawPos.y - elipseSize.y, drawPos.z), new Vector3(-(elipseSize.x), -(elipseSize.y / 2), 0) + drawPos, new Vector3(-(elipseSize.x / 2), -(elipseSize.y), 0) + drawPos, col, Texture2D.whiteTexture, size);
		//X-Z Ring
		Handles.DrawBezier(new Vector3(drawPos.x + elipseSize.x, drawPos.y, drawPos.z), new Vector3(drawPos.x, drawPos.y, drawPos.z + elipseSize.z), new Vector3((elipseSize.x), 0, (elipseSize.z / 2))   + drawPos, new Vector3((elipseSize.x / 2), 0, (elipseSize.z))   + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x - elipseSize.x, drawPos.y, drawPos.z), new Vector3(drawPos.x, drawPos.y, drawPos.z + elipseSize.z), new Vector3(-(elipseSize.x), 0, (elipseSize.z / 2))  + drawPos, new Vector3(-(elipseSize.x / 2), 0, (elipseSize.z))  + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x + elipseSize.x, drawPos.y, drawPos.z), new Vector3(drawPos.x, drawPos.y, drawPos.z - elipseSize.z), new Vector3((elipseSize.x), 0, -(elipseSize.z / 2))  + drawPos, new Vector3((elipseSize.x / 2), 0, -(elipseSize.z))  + drawPos, col, Texture2D.whiteTexture, size);
		Handles.DrawBezier(new Vector3(drawPos.x - elipseSize.x, drawPos.y, drawPos.z), new Vector3(drawPos.x, drawPos.y, drawPos.z - elipseSize.z), new Vector3(-(elipseSize.x), 0, -(elipseSize.z / 2)) + drawPos, new Vector3(-(elipseSize.x / 2), 0, -(elipseSize.z)) + drawPos, col, Texture2D.whiteTexture, size);
	}
#endif
}
