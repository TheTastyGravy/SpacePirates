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
	public GameObject explosionPrefab;
	public Transform magicCollider;

	private Transform trans;
	private Transform shipTrans;
    private ObjectPool pool;
    private ObjectPool explosionPool;
	private bool hasHitShip;



	void Awake()
	{
		// Allign collider with camera
		magicCollider.rotation = Camera.main.transform.rotation;
		trans = transform;
		shipTrans = ShipManager.Instance.transform;
        pool = ObjectPool.GetPool("Missile Pool");
        explosionPool = ObjectPool.GetPool("Explosion Pool");
    }

    void OnEnable()
    {
        hasHitShip = false;
        pool.Return(gameObject, 10);
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
		// Prevent multiple collisions being registered
		if (hasHitShip)
			return;
		hasHitShip = true;

		if (collision.gameObject.CompareTag("TurretProjectile"))
		{
            // We have been hit by a turret
            ObjectPool.GetPool("Projectile Pool").Return(collision.gameObject);
			health--;
			if (health <= 0)
			{
                pool.Return(gameObject);
                // This is a different explosion effect that is only used here, so dont use object pooling
				GameObject obj = Instantiate(explosionPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
				Destroy(obj, 3);
			}
		}
		else
		{
			// If we hit something else, damage the ship
			ShipManager.Instance.DamageShipAtPosition(transform.position);
            pool.Return(gameObject);
            // Create explosion
            GameObject explosion = explosionPool.GetInstance();
            explosion.transform.SetPositionAndRotation(collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            explosionPool.Return(explosion, 3);
        }
	}
}
