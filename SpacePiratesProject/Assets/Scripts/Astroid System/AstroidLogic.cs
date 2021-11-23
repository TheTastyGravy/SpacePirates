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
    public float minRotationSpeed = 0.25f;
    public float maxRotationSpeed = 0.75f;
    [Tooltip("Update the capsule collider to try and get it to contain the astroid. Error prone")]
    public bool updateCollider = true;

    [Header("Gameplay")]
    [Tooltip("How many times it can be hit by a turret before being destroied")]
    public int health = 1;
    [Tooltip("This is how fast the asteroid moves in space")]
    public float speed = 10;

    [Space]
    [Tooltip("Explosion prefab scale when the asteroid is destroied by a turret")]
    public float turretExplosionScale = 0.7f;
    public Transform magicCollider;

    private Transform trans;
    private ObjectPool pool;
    private Vector3 direction;
    private Transform visualTrans;
    private Vector3 visualCenter;
    private float rotAngle;
    private Vector3 rotAxis;
    private int currentHealth;
    private float currentSpeed;
    private bool hasHitShip = false;
    // Asteroids use object pooling
    private bool hasBeenSetup = false;
    private float timePassed;



    internal void Setup(Vector3 startPos, Vector3 direction, float delay)
    {
        if (!hasBeenSetup)
        {
            hasBeenSetup = true;
            // Allign collider with camera
            magicCollider.rotation = Camera.main.transform.rotation;
            trans = transform;
            pool = ObjectPool.GetPool("Asteroid Pool");
        }

        currentHealth = health;
        currentSpeed = 0;
        hasHitShip = false;
        timePassed = 0;
        trans.position = startPos;
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
        visualTrans = selected.transform;
        selected.SetActive(true);
        Renderer renderer = selected.GetComponent<Renderer>();

        // Apply random rotation about the center of the mesh
        visualCenter = renderer.bounds.center;
        Random.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        visualTrans.RotateAround(visualCenter, axis, angle);
        // Scale by random factor
        visualTrans.localScale *= Random.Range(minScaleFactor, maxScaleFactor);

        // Get a random angle and axis for rotation
        rotAngle = 360f * Random.Range(minRotationSpeed, maxRotationSpeed);
        rotAxis = Random.onUnitSphere;
        // Convert center point from world to local of the base transform
        visualCenter -= trans.position;

        if (updateCollider)
        {
            // Update the collider to contain the astroid. The radius is approxamate due to 
            // how renderer.bounds works. It is an AABB in world space that contains the bounds 
            // of the mesh, which is stored in local space. The result is an AABB containing an 
            // OBB, causing renderer.bounds to be larger than the actual object when rotated.
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            col.radius = Mathf.Max(renderer.bounds.extents.x, renderer.bounds.extents.z);
            col.center = renderer.bounds.center - trans.position;
        }
    }

    private void Delay()
    {
        currentSpeed = speed;
        timePassed = 0;
    }

    void Update()
    {
        // Update position
        trans.position += currentSpeed * Time.deltaTime * direction;
        // Rotate visual astroid
        visualTrans.RotateAround(visualCenter + trans.position, rotAxis, rotAngle * Time.deltaTime);

        timePassed += Time.deltaTime;
        if (timePassed >= 10)
            pool.Return(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with the chase ship, since it handles being hit by asteroids
        if (collision.gameObject.layer == LayerMask.NameToLayer("ChaseShip"))
            return;

        if (collision.gameObject.CompareTag("TurretProjectile"))
        {
            // We have been hit by a turret
            ObjectPool.GetPool("Projectile Pool").Return(collision.gameObject);
            currentHealth--;
            if (currentHealth <= 0)
            {
                pool.Return(gameObject);
                // Create explosion
                ObjectPool explosionPool = ObjectPool.GetPool("Explosion Pool");
                Transform explosionTrans = explosionPool.GetInstance().transform;
                explosionTrans.SetPositionAndRotation(trans.position, Random.rotation);
                explosionTrans.localScale = new Vector3(turretExplosionScale, turretExplosionScale, turretExplosionScale);
                explosionPool.Return(explosionTrans.gameObject, 3);
            }
        }
        else
        {
            // Prevent multiple collisions being registered
            if (hasHitShip)
                return;
            hasHitShip = true;

            ShipManager.Instance.DamageShipAtPosition(trans.position);
            // Destroy this asteroid and create an explosion
            pool.Return(gameObject);
            ObjectPool explosionPool = ObjectPool.GetPool("Explosion Pool");
            GameObject explosion = explosionPool.GetInstance();
            explosion.transform.SetPositionAndRotation(collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            explosionPool.Return(explosion, 3);
        }
    }
}
