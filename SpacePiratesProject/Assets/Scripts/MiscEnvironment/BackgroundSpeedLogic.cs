using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpeedLogic : MonoBehaviour
{
    public BackgroundController background;
    public MeshRenderer backgroundBase;
    public GameObject plasmaStormEffectPrefab;
    public Color storm_backgroundColor;
    public float storm_timeBetween = 0.5f;
    public float storm_baseSpeed = 1;
    public float storm_fadeOutTime = 1.5f;
    [Space]
    public float minSpeedModifier = 0.1f;
    public float maxSpeedModifier = 1;

    private Color backgroundBaseColor;
    private float inverseMaxSpeed = 0;
    private float speedModifier = 1;
    private Coroutine eventRoutine = null;
    private Level.Event.Type currentEvent = Level.Event.Type.None;

    private List<Transform> objects = new List<Transform>();
    // Past random values generated. Used for weighting
    List<float> values = new List<float>();



    void Start()
    {
        backgroundBaseColor = backgroundBase.material.color;
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
            if (obj == null)
            {
                toDestroy.Add(obj);
                continue;
            }

            obj.position += obj.forward * Time.deltaTime * speedModifier * storm_baseSpeed;
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
            if (obj != null)
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
                StartCoroutine(SetBackgroundColor(storm_backgroundColor, 1));
                break;
            case Level.Event.Type.ShipAttack:

                break;
            case Level.Event.Type.None:
                StartCoroutine(SetBackgroundColor(backgroundBaseColor, 1));
                break;
		}

        // When a plasma storm ends, fade out the remaining clouds
        if (currentEvent == Level.Event.Type.PlasmaStorm)
        {
            FadeOutObjects(storm_fadeOutTime);
        }

        currentEvent = eventType;
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
            Vector3 dir = cam.transform.TransformPoint(new Vector2(0.5f, 0.45f)) - cam.transform.TransformPoint(new Vector2(0, 0));
            spawnLinePos += dir.normalized * 7.5f;
            // Push down and up to spawn behind the player ship
            spawnLinePos += new Vector3(-1, -1, 0) * 10;

            spawnLineDir = Vector3.right;
            rotation = Quaternion.LookRotation(Vector3.back, -cam.transform.forward);
        }

        while (true)
		{
            Vector3 spawnPos = spawnLinePos + spawnLineDir * GetRandomValue(25)/*Random.Range(-15f, 15f)*/;
            GameObject effectObj = Instantiate(plasmaStormEffectPrefab, spawnPos, rotation);
            objects.Add(effectObj.transform);

            yield return new WaitForSeconds(storm_timeBetween / speedModifier);
		}
	}

    private void FadeOutObjects(float time)
    {
        foreach (var obj in objects)
        {
            if (obj.TryGetComponent(out RunLightning runLightning))
                runLightning.enabled = false;

            ParticleSystem particleSystem = obj.GetComponentInChildren<ParticleSystem>();
            if (particleSystem != null)
            {
                // Disable emission
                ParticleSystem.EmissionModule emission = particleSystem.emission;
                emission.enabled = false;

                // Set the lifetime of all particles
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
                int count = particleSystem.GetParticles(particles);
                for (int i = 0; i < count; i++)
                {
                    particles[i].remainingLifetime = particles[i].remainingLifetime / particles[i].startLifetime * time;
                    particles[i].startLifetime = time;
                }
                particleSystem.SetParticles(particles);

                // Destroy the object when the particles are gone
                Destroy(obj.gameObject, time);
            }
        }
    }

    private IEnumerator SetBackgroundColor(Color color, float time)
    {
        Color startColor = backgroundBase.material.color;
        float t = 0;
        while (t < 1)
        {
            backgroundBase.material.color = Color.Lerp(startColor, color, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        backgroundBase.material.color = color;
    }

    float GetRandomValue(float range)
    {
        //good luck
        float value = 0;
        if (values.Count > 0)
        {
            float total = 0;
            for (int i = 0; i < values.Count; i++)
            {
                total += values[i] * (i < 3 ? 1.25f : 1);
            }
            value = -(total / values.Count);
        }
        if (values.Count > 0 && Mathf.Abs(Mathf.Abs(value) - Mathf.Abs(values[values.Count - 1])) < 0.15f)
        {
            value -= values[values.Count - 1] * 0.3f;
        }
        //random variance
        value += Random.value * 2 - 1;

        //convert from -2|2 to 0|1
        float prob = 0.5f + (value / 4f);
        prob = Mathf.Clamp(prob, 0, 0.999f);

        float randomValue = NormInv(prob);
        randomValue = Mathf.Clamp(randomValue, -1, 1);

        //update total
        values.Add(randomValue);
        if (values.Count > 8)
            values.RemoveAt(0);

        return randomValue * range;
    }

    public float NormInv(float probability)
    {
        // Define variables used in intermediate steps
        float q = 0f;
        float r = 0f;
        float x = 0f;

        // Coefficients in rational approsimations.
        float[] a = new float[] { -3.969683028665376e+01f, 2.209460984245205e+02f, -2.759285104469687e+02f, 1.383577518672690e+02f, -3.066479806614716e+01f, 2.506628277459239e+00f };
        float[] b = new float[] { -5.447609879822406e+01f, 1.615858368580409e+02f, -1.556989798598866e+02f, 6.680131188771972e+01f, -1.328068155288572e+01f };
        float[] c = new float[] { -7.784894002430293e-03f, -3.223964580411365e-01f, -2.400758277161838e+00f, -2.549732539343734e+00f, 4.374664141464968e+00f, 2.938163982698783e+00f };
        float[] d = new float[] { 7.784695709041462e-03f, 3.224671290700398e-01f, 2.445134137142996e+00f, 3.754408661907416e+00f };

        // Define break-points
        float pLow = 0.02425f;
        float pHigh = 1f - pLow;

        // Verify that probability is between 0 and 1 (noninclusinve), and if not, make between 0 and 1
        if (probability <= 0f)
        {
            probability = 0.00001f;
        }
        else if (probability >= 1f)
        {
            probability = 1f - 0.00001f;
        }

        // Rational approximation for lower region.
        if (probability < pLow)
        {
            q = Mathf.Sqrt(-2f * Mathf.Log(probability));
            x = (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) / ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1f);
        }

        // Rational approximation for central region.
        if (pLow <= probability && probability <= pHigh)
        {
            q = probability - 0.5f;
            r = q * q;
            x = (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q / (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1f);
        }

        // Rational approximation for upper region.
        if (pHigh < probability)
        {
            q = Mathf.Sqrt(-2 * Mathf.Log(1f - probability));
            x = -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) / ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1f);
        }

        return x;
    }

    //public void OnDrawGizmos()
    //{
    //    Vector3 spawnLineDir;
    //    Vector3 spawnLinePos;
    //    // Get values for object creation
    //    {
    //        Camera cam = Camera.main;
    //        Ray ray = cam.ViewportPointToRay(new Vector2(1, 1));    //top right corner
    //        new Plane(Vector3.up, 0).Raycast(ray, out float enter);
    //        spawnLinePos = ray.GetPoint(enter);
    //
    //        // Add offset to make sure they are spawned off screen
    //        Vector3 dir = cam.transform.TransformPoint(new Vector2(0.5f, 0.45f)) - cam.transform.TransformPoint(new Vector2(0, 0));
    //        spawnLinePos += dir.normalized * 7.5f;
    //        // Push down and up to spawn behind the player ship
    //        spawnLinePos += new Vector3(-1, -1, 0) * 10;
    //
    //        spawnLineDir = Vector3.right;
    //    }
    //
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(spawnLinePos + spawnLineDir * -25, spawnLinePos + spawnLineDir * 25);
    //    Gizmos.DrawSphere(spawnLinePos, 0.5f);
    //}
}