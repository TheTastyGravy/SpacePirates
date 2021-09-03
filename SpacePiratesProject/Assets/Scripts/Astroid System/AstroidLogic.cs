using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AstroidLogic : MonoBehaviour
{
	[Tooltip("How many times it can be hit by a turret before being destroied")]
	public int health = 1;
	[Tooltip("This is how fast the asteroid moves in space")]
	public float speed = 10;

	private Vector3 direction;
	private bool hasHitShip = false;
	private float currentSpeed = 0;



	internal void Setup(Vector3 startPos, Vector3 direction, float delay)
	{
		transform.position = startPos;
		this.direction = direction.normalized;

		Invoke(nameof(Delay), delay);
	}

	private void Delay()
	{
		currentSpeed = speed;
		Destroy(gameObject, 5);
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
		}
	}
}
