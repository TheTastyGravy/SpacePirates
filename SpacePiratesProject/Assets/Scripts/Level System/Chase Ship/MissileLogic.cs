using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MissileLogic : MonoBehaviour
{
	[Tooltip("How many times it can be hit by a turret before being destroied")]
	public int health = 1;
	public float speed = 10;
	[Space]
	public GameObject explosion;
	public Transform magicCollider;


	private bool hasHitShip = false;



	void Start()
	{
		// Allign collider with camera
		magicCollider.rotation = Camera.main.transform.rotation;
		Destroy(gameObject, 10);
	}

	void Update()
	{
		// Update position
		transform.position += speed * Time.deltaTime * transform.forward;
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
