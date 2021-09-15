using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AstroidLogic : MonoBehaviour
{
	[Header("Graphics")]
	public GameObject[] visuals;
	public float minScaleFactor = 0.8f;
	public float maxScaleFactor = 1.2f;
	[Tooltip("Update the capsule collider to try and get it to contain the astroid. Error prone")]
	public bool updateCollider = true;

	[Header("Gameplay")]
	[Tooltip("How many times it can be hit by a turret before being destroied")]
	public int health = 1;
	[Tooltip("This is how fast the asteroid moves in space")]
	public float speed = 10;

	[Space]
	public GameObject explosion;

	private Vector3 direction;
	private bool hasHitShip = false;
	private float currentSpeed = 0;



	internal void Setup(Vector3 startPos, Vector3 direction, float delay)
	{
		transform.position = startPos;
		this.direction = direction.normalized;
		SetupVisuals();

		Invoke(nameof(Delay), delay);
	}

	private void SetupVisuals()
	{
		// Disable all visuals
		foreach (var obj in visuals)
		{
			obj.SetActive(false);
		}
		// Select a random visual to use
		GameObject selected = visuals[Random.Range(0, visuals.Length)];
		selected.SetActive(true);
		Renderer renderer = selected.GetComponent<Renderer>();

		// Apply random rotation about the center of the mesh
		Vector3 center = renderer.bounds.center;
		Random.rotation.ToAngleAxis(out float angle, out Vector3 axis);
		selected.transform.RotateAround(center, axis, angle);
		// Scale by random factor
		selected.transform.localScale *= Random.Range(minScaleFactor, maxScaleFactor);


		if (updateCollider)
		{
			// Update the collider to contain the astroid. The radius is approxamate due to 
			// how renderer.bounds works. It is an AABB in world space that contains the bounds 
			// of the mesh, which is stored in local space. The result is an AABB containing an 
			// OBB, causing renderer.bounds to be larger than the actual object when rotated.
			CapsuleCollider col = GetComponent<CapsuleCollider>();
			col.radius = Mathf.Max(renderer.bounds.extents.x, renderer.bounds.extents.z);
			col.center = renderer.bounds.center - transform.position;
		}
	}

	private void Delay()
	{
		currentSpeed = speed;
		Destroy(gameObject, 10);
	}

	void Update()
	{
		transform.position += currentSpeed * Time.deltaTime * direction;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("TurretProjectile"))
		{
			// We have been hit by a turret
			Destroy(collision.gameObject);
			health--;
			if (health <= 0)
			{
				Destroy(gameObject);
			}
		}
		else
		{
			// Prevent multiple collisions being registered
			if (hasHitShip)
				return;

			// If we hit something else, damage the ship
			ShipManager.Instance.DamageShipAtPosition(transform.position);
			hasHitShip = true;
			Destroy(gameObject);

			GameObject obj = Instantiate(explosion, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
			Destroy(obj, 3);
		}
	}
}
