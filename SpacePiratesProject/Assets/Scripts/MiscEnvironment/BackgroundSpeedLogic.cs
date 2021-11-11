using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpeedLogic : MonoBehaviour
{
    public BackgroundController background;
    public MeshRenderer backgroundBase;
    [Space]
    public GameObject asteroidFieldEffectPrefab;
    public GameObject plasmaStormEffectPrefab;
    public Color storm_backgroundColor;
    [Space]
    public float minSpeedModifier = 0.1f;
    public float maxSpeedModifier = 1;

    private Color backgroundBaseColor;
    private float inverseMaxSpeed = 0;
    private float speedModifier = 1;
    private AsteroidFieldEffect asteroidEffect;
    private PlasmaStormEffect plasmaStormEffect;



    void Start()
    {
        backgroundBaseColor = backgroundBase.material.color;
        EventManager.Instance.OnEventChange += OnEventChange;
        Invoke(nameof(Init), 0);
    }

	private void Init()
	{
        inverseMaxSpeed = 1 / ShipManager.Instance.GetMaxSpeed();

        plasmaStormEffect = Instantiate(plasmaStormEffectPrefab, transform).GetComponent<PlasmaStormEffect>();
        asteroidEffect = Instantiate(asteroidFieldEffectPrefab, transform).GetComponent<AsteroidFieldEffect>();
    }

    private void OnDestroy()
    {
        if (plasmaStormEffect != null)
            Destroy(plasmaStormEffect.gameObject);
        if (asteroidEffect != null)
            Destroy(asteroidEffect.gameObject);
    }

    void LateUpdate()
    {
        // Update value
        speedModifier = Mathf.Lerp(minSpeedModifier, maxSpeedModifier, ShipManager.Instance.GetShipSpeed() * inverseMaxSpeed);

        // Set background scroll speed
        background.speedMultiplier = speedModifier;
        //Set speed for event effects
        asteroidEffect.speedModifier = speedModifier;
        plasmaStormEffect.speedModifier = speedModifier;
    }

    private void OnEventChange(Level.Event.Type eventType, EventManager.EventStage stage)
	{
        // Event enter
        if (stage == EventManager.EventStage.INIT)
        {
            switch (eventType)
            {
                case Level.Event.Type.AstroidField:
                    asteroidEffect.StartEffect();
                    break;
                case Level.Event.Type.PlasmaStorm:
                    plasmaStormEffect.StartEffect();
                    StartCoroutine(SetBackgroundColor(storm_backgroundColor, 1));
                    break;
            }
        }
        // Event exit
        else if (stage == EventManager.EventStage.END)
        {
            StartCoroutine(SetBackgroundColor(backgroundBaseColor, 1));

            // When a plasma storm ends, fade out the remaining clouds
            if (eventType == Level.Event.Type.PlasmaStorm)
            {
                plasmaStormEffect.StopEffect();
            }
            if (eventType == Level.Event.Type.AstroidField)
            {
                asteroidEffect.StopEffect();
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
}
