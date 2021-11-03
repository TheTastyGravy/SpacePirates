using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpeedLogic : MonoBehaviour
{
    public BackgroundController background;
    public GameObject plasmaStormEffectPrefab;
    public float storm_timeBetween = 0.5f;
    public float storm_baseSpeed = 1;
    [Space]
    public float minSpeedModifier = 0.1f;
    public float maxSpeedModifier = 1;

    private float inverseMaxSpeed = 0;
    private float speedModifier = 1;
    private Coroutine eventRoutine = null;

    private List<Transform> objects = new List<Transform>();



    void Start()
    {
        EventManager.Instance.OnEventChange += OnEventChange;
        Invoke(nameof(Init), 0);
    }

	private void Init()
	{
        inverseMaxSpeed = 1 / ShipManager.Instance.GetMaxSpeed();
    }

	void LateUpdate()
    {
        // Update value
        speedModifier = Mathf.Lerp(minSpeedModifier, maxSpeedModifier, ShipManager.Instance.GetShipSpeed() * inverseMaxSpeed);

        // Set background scross speed
        background.speedMultiplier = speedModifier;
        // Move objects (plasma storm clouds)
        List<Transform> toDestroy = new List<Transform>();
        foreach (var obj in objects)
		{
            obj.position += obj.forward * speedModifier * storm_baseSpeed;
            // Destroy objects when they go far enough
            if (Vector3.Dot(obj.position, obj.forward) > 20)
			{
                toDestroy.Add(obj);
                continue;
            }
        }
        foreach (var obj in toDestroy)
		{
            objects.Remove(obj);
            Destroy(obj.gameObject);
        }
    }

    private void OnEventChange(Level.Event.Type eventType)
	{
        if (eventRoutine != null)
            StopCoroutine(eventRoutine);

        switch (eventType)
		{
            case Level.Event.Type.AstroidField:

                break;
            case Level.Event.Type.PlasmaStorm:
                eventRoutine = StartCoroutine(PlasmaStormEffect());
                break;
            case Level.Event.Type.ShipAttack:

                break;
		}
	}

    private IEnumerator PlasmaStormEffect()
	{
        Vector3 spawnLineDir;
        Vector3 spawnLinePos;
        Quaternion rotation;
        // Get values for object creation
        {
            Camera cam = Camera.main;
            Ray ray = cam.ViewportPointToRay(new Vector2(1, 1));    //top right corner
            new Plane(Vector3.up, 0).Raycast(ray, out float enter);
            spawnLinePos = ray.GetPoint(enter);

            // Add offset to make sure they are spawned off screen
            Vector3 dir = cam.transform.TransformPoint(new Vector2(0.5f, 0.5f)) - cam.transform.position;
            spawnLinePos += dir.normalized * 5;
            // Push down and up to spawn behind the player ship
            spawnLinePos += new Vector3(-1, -1, 0) * 10;

            spawnLineDir = Vector3.right;
            rotation = Quaternion.LookRotation(Vector3.back, -cam.transform.forward);
        }

        while (true)
		{
            Vector3 spawnPos = spawnLinePos + spawnLineDir * Random.Range(-15f, 15f);
            GameObject effectObj = Instantiate(plasmaStormEffectPrefab, spawnPos, rotation);
            objects.Add(effectObj.transform);

            yield return new WaitForSeconds(storm_timeBetween * (1 / speedModifier));
		}
	}
}
