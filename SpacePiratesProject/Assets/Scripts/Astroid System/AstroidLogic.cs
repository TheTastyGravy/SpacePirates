using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AstroidLogic : MonoBehaviour
{
	[Tooltip("How many times it can be hit by a turret before being destroied")]
	public int health = 1;

	private Vector3 direction;
	private float speed = 10;



	internal void Setup(Vector3 startPos, Vector3 direction)
	{
		transform.position = startPos;
		this.direction = direction.normalized;
	}


	void Update()
	{
		transform.position += speed * Time.deltaTime * direction;
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
			// If we hit something else, damage the ship
			ShipManager.Instance.DamageShipAtPosition(transform.position);
			Destroy(gameObject);
		}
	}
}
