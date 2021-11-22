using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MissileLogic : MonoBehaviour
{
	[Tooltip("How many times it can be hit by a turret before being destroied")]
	public int health = 1;
	public float speed = 10;
	public float steeringSpeed = 30;
	public float angleFactorMultiplier = 1;
	[Space]
	public GameObject explosion, otherExplosion;
	public Transform magicCollider;

	private Transform trans;
	private Transform shipTrans;
	private bool hasHitShip = false;



	void Start()
	{
		// Allign collider with camera
		magicCollider.rotation = Camera.main.transform.rotation;
		Destroy(gameObject, 10);
		trans = transform;
		shipTrans = ShipManager.Instance.transform;
	}

	void Update()
	{
		// Rotate the forward direction to try andfact the player ship
		Vector3 dir = shipTrans.position - trans.position;
		float angleFactor = Mathf.Abs(Vector3.Angle(trans.forward, dir) * Mathf.Deg2Rad) * angleFactorMultiplier;
		dir = Vector3.RotateTowards(trans.forward, dir, steeringSpeed * Mathf.Deg2Rad * Time.deltaTime * angleFactor, 0);
		dir.y = 0;
		trans.forward = dir;
		// Update position
		trans.position += speed * Time.deltaTime * trans.forward;
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
				GameObject obj = Instantiate(otherExplosion, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
				Destroy(obj, 3);
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
