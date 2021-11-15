using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ShipManager : Singleton<ShipManager>
{
    [Header("Oxygen")]
    public float maxOxygenLevel = 100;
    public float passiveOxygenLoss = 1;
    public float timeToGameOver = 0.5f;
    [Header("Avoidance")]
    [Range(0,1)]
    public float maxAvoidance = 0.25f;
    [Space]
    public float fadeTime = 1;
    public float fadeDelay = 1;
    public Renderer[] roof;
    public Renderer[] body;
    public Color bodyColor;
    [Space]
    public MeshFilter mainMesh;
    public GameObject explosionPrefab;
    public float timeBetweenExplosions = 0.2f;
    [Space]
    public float wanderFrequency = 1;
    public float wanderDist = 0.75f;
    public float wanderSpeed = 3;
    public float wanderAcceleration = 1;
    [Space]
    public float damageHapticsTime = 0.2f;
    public float damageHapticsPower = 0.3f;
    public EventReference impactEvent;

    private ReactorStation[] reactors;
    public ReactorStation Reactor => reactors.Length > 0 ? reactors[0] : null;
    private EngineStation[] engines;
    private RoomManager[] rooms;

    // Event for game over due to running out of oxygen
    public BasicDelegate OnZeroOxygen;
    private float oxygenLevel;
    public float OxygenLevel => oxygenLevel;
    private OxygenBar oxygenBar;
    [HideInInspector]
    public float oxygenDrain = 0;
    private float gameOverTimmer;
    private bool isCheatActive = false;

    private bool useWander = false;
    private float wanderTimePassed = 0;
    private Vector3 wanderPos;
    private Vector3 velocity = Vector3.zero;

    private float[] cumulativeSizes;
    private float total;



    void Start()
    {
        reactors = GetComponentsInChildren<ReactorStation>();
        engines = GetComponentsInChildren<EngineStation>();
        rooms = GetComponentsInChildren<RoomManager>();

        foreach (var obj in roof)
        {
            obj.material.color = Color.white;
        }

        oxygenLevel = maxOxygenLevel;
        oxygenBar = FindObjectOfType<OxygenBar>();
        oxygenBar.MaxValue = maxOxygenLevel;
        oxygenBar.value = maxOxygenLevel;

        // Nothing to see here...
        Character.OnCheatActivated += () => isCheatActive = true;
    }

    public void BeginGame()
    {
        StartCoroutine(FadeShip(fadeTime, true));
        foreach (var obj in reactors)
        {
            obj.enabled = true;
        }
        foreach (var obj in engines)
        {
            obj.enabled = true;
        }
        useWander = true;
    }

    public void DamageShipAtPosition(Vector3 position)
    {
        HoleData holeData = new HoleData
        {
            hitPos = position,
            sqrDistance = Mathf.Infinity
        };

        // Find the closest hole positon, then damage the room
        foreach (var room in rooms)
        {
            room.FindClosestHolePos(ref holeData);
        }
        holeData.room.DamageRoom(holeData);
        // Play effects
        RuntimeManager.PlayOneShot(impactEvent);
        Player.PulseAllHaptics(damageHapticsTime, damageHapticsPower);
        CameraManager.Instance.Shake();
    }

    public float GetShipSpeed()
	{
        // Its possible for this function to be called before start
        if (engines == null)
            return 0;

        // Accumulate speed from engines
        float speed = 0;
        foreach (var engine in engines)
		{
            speed += engine.CurrentSpeed;
		}

        return speed;
	}

    public float GetMaxSpeed()
    {
        if (engines == null)
            return 0;

        // Accumulate speed from engines
        float speed = 0;
        foreach (var engine in engines)
        {
            speed += engine.MaxSpeed;
        }
        return speed;
    }

    public float GetShipAvoidance()
	{
        int active = 0;
        foreach (var obj in engines)
		{
            if (obj.IsTurnedOn)
                active++;
		}

        return maxAvoidance * (active / engines.Length);
	}

    public DamageStation GetRandomActiveStation()
	{
        List<DamageStation> stations = new List<DamageStation>();
        // Add active reactors
        foreach (var obj in reactors)
		{
            if (obj.IsTurnedOn)
			{
                stations.Add(obj.Damage);
			}
		}
        // Add active engines
        foreach (var obj in engines)
		{
            if (obj.IsTurnedOn)
			{
                stations.Add(obj.Damage);
			}
		}

        if (stations.Count > 0)
		{
            return stations[Random.Range(0, stations.Count)];
        }
		else
		{
            return null;
		}
	}

    public IEnumerator FadeShip(float time, bool fadeOut)
    {
        float t = 0;
        while (t < time)
        {
            float val = t / time;
            val = fadeOut ? val : 1 - val;
            foreach (var obj in roof)
            {
                obj.material.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), val);
            }
            foreach (var obj in body)
            {
                obj.material.color = Color.Lerp(Color.white, bodyColor, val);
            }

            t += Time.deltaTime;
            yield return null;
        }

        foreach (var obj in roof)
        {
            obj.material.color = new Color(1, 1, 1, fadeOut ? 0 : 1);
        }
        foreach (var obj in body)
        {
            obj.material.color = fadeOut ? bodyColor : Color.white;
        }
    }

    void Update()
    {
        UpdateOxygen();

        // Random wander
        if (useWander)
        {
            wanderTimePassed += Time.deltaTime;
            if (wanderTimePassed >= wanderFrequency)
            {
                wanderTimePassed = 0;
                // Get new wander pos
                wanderPos = Random.insideUnitSphere * wanderDist;
                wanderPos.y = 0;
            }
            // Adjust velocity with a sort of steering force, and apply to position
            velocity = Vector3.Lerp(velocity, (wanderPos - transform.position).normalized * wanderSpeed, Time.deltaTime * wanderAcceleration);
            velocity.y = 0;
            transform.position += velocity * Time.deltaTime;
        }
    }

    private void UpdateOxygen()
	{
        if (isCheatActive)
		{
            //hacker man
            oxygenLevel = maxOxygenLevel;
            oxygenBar.value = oxygenLevel;
            oxygenDrain = 0;
            return;
        }

        // Find the total loss rate
        float oxygenLossRate = passiveOxygenLoss;
        foreach (var room in rooms)
        {
            oxygenLossRate += room.localOxygenDrain;
        }
        // Decrease level by loss rate
        oxygenLevel -= oxygenLossRate * Time.deltaTime;

        // Find the regen rate and apply it
        float oxygenRegenRate = 0;
        foreach (var reactor in reactors)
		{
            oxygenRegenRate += reactor.CurrentOxygenRegen;
		}
        oxygenLevel += oxygenRegenRate * Time.deltaTime;
        if (oxygenLevel > maxOxygenLevel)
            oxygenLevel = maxOxygenLevel;

        if (oxygenLevel <= 0)
        {
            oxygenLevel = 0;
            // Start timmer to game over
            gameOverTimmer += Time.deltaTime;
            if (gameOverTimmer >= timeToGameOver)
			{
                OnZeroOxygen.Invoke();
			}
        }
		else
		{
            gameOverTimmer = 0;
		}

        // Update the UI
        oxygenBar.value = oxygenLevel;
        oxygenDrain = oxygenRegenRate - oxygenLossRate;
    }


    public void MoveForward(float time, float distance)
	{
        Camera.main.transform.parent = null;
        Vector3 endPos = transform.position + transform.forward * distance;
        StartCoroutine(MoveShip(time, endPos));
	}

    private IEnumerator MoveShip(float time, Vector3 end)
	{
        Vector3 start = transform.position;
        float t = 0;
        while (t < time)
		{
            transform.position = Vector3.Lerp(start, end, t / time);
            t += Time.deltaTime;
            yield return null;
		}
	}


    public void ExplodeShip()
    {
        if (mainMesh != null && explosionPrefab != null)
		{
            // Setup for GetRandomPointOnMesh()
            float[] sizes = GetTriSizes(mainMesh.mesh.triangles, mainMesh.mesh.vertices);
            cumulativeSizes = new float[sizes.Length];
            total = 0;
            for (int i = 0; i < sizes.Length; i++)
            {
                total += sizes[i];
                cumulativeSizes[i] = total;
            }

            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        while (true)
        {
            // Meshes are concidered in local space, so it needs to be converted
            Vector3 point = mainMesh.transform.localToWorldMatrix * GetRandomPointOnMesh();
            // Create explosion effect, and destroy it after its done
            GameObject explosionInstance = Instantiate(explosionPrefab, point + mainMesh.transform.position, Quaternion.LookRotation(point - mainMesh.mesh.bounds.center, Random.onUnitSphere), transform);
            Destroy(explosionInstance, 2);

            yield return new WaitForSeconds(timeBetweenExplosions);
        }
    }

    // Get a random position on the surface of a mesh
    private Vector3 GetRandomPointOnMesh()
    {
        Mesh mesh = mainMesh.mesh;

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
