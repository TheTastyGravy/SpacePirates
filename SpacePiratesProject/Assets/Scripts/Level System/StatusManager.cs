 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusManager : Singleton<StatusManager>
{
    [System.Serializable]
    public struct StringGroup
    {
        [SerializeField]
        private string[] strings;

        public string String => strings[Random.Range(0, strings.Length)];
    }



    public TextMeshProUGUI text;
    public float refreshRate = 1;
    public float flashTime = 0.5f;

    [Header("Normal")]
    public StringGroup general;
    public StringGroup gameStart;
    [Space]
    public StringGroup reactorDamaged;
    public StringGroup reactorDisabled;
    [Space]
    public StringGroup generalRepair;
    public StringGroup generalRefuel;
    [Space]
    public StringGroup useEngines;
    public StringGroup useTurrets;

    [Header("Events")]
    public float genericChance = 0.5f;
    public StringGroup genericEvent;
    [Space]
    public StringGroup asteroidEnter;
    public StringGroup asteroidDurring;
    public StringGroup asteroidExit;
    [Space]
    public StringGroup stormEnter;
    public StringGroup stormDurring;
    public StringGroup stormExit;
    [Space]
    public StringGroup shipEnter;
    public StringGroup shipDurring;
    public StringGroup shipExit;

    private Level.Event.Type currentEvent = Level.Event.Type.None;
    private Level.Event.Type lastEvent = Level.Event.Type.None;
    private bool eventActive = false;
    private bool inEventLoop = false;
    private EngineStation[] engines;
    private TurretStation[] turrets;
    private float timePassed = 0;



    void Start()
    {
        text.text = gameStart.String;
        EventManager.Instance.OnEventChange += OnEventChange;
        Invoke(nameof(Init), 0.1f);
    }

	private void Init()
	{
        engines = ShipManager.Instance.gameObject.GetComponentsInChildren<EngineStation>();
        turrets = ShipManager.Instance.gameObject.GetComponentsInChildren<TurretStation>();
    }

	void OnDestroy()
	{
        if (EventManager.Instance != null)
            EventManager.Instance.OnEventChange -= OnEventChange;
    }

	void Update()
	{
        timePassed += Time.deltaTime;
        if (timePassed >= refreshRate)
        {
            timePassed = 0;
        }
        else
        {
            return;
        }

        //  -----   EVENTS   -----
        if (eventActive)
        {
            if (!inEventLoop)
            {
                if (currentEvent == Level.Event.Type.None)
                {
                    //exiting
                    switch (lastEvent)
                    {
                        case Level.Event.Type.AstroidField:
                            text.text = asteroidExit.String;
                            break;
                        case Level.Event.Type.PlasmaStorm:
                            text.text = stormExit.String;
                            break;
                        case Level.Event.Type.ShipAttack:
                            text.text = shipExit.String;
                            break;
                    }
                    eventActive = false;
                }
                else
                {
                    //entering
                    switch (currentEvent)
                    {
                        case Level.Event.Type.AstroidField:
                            text.text = asteroidEnter.String;
                            break;
                        case Level.Event.Type.PlasmaStorm:
                            text.text = stormEnter.String;
                            break;
                        case Level.Event.Type.ShipAttack:
                            text.text = shipEnter.String;
                            break;
                    }
                    StartCoroutine(TextFlash());
                }

                inEventLoop = true;
            }
            else
            {
                if (Random.Range(0f, 1f) < genericChance)
                {
                    text.text = genericEvent.String;
                }
                else
                {
                    switch (currentEvent)
                    {
                        case Level.Event.Type.AstroidField:
                            text.text = asteroidDurring.String;
                            break;
                        case Level.Event.Type.PlasmaStorm:
                            text.text = stormDurring.String;
                            break;
                        case Level.Event.Type.ShipAttack:
                            text.text = shipDurring.String;
                            break;
                    }
                }
            }

            return;
        }

        
        //  -----   GENERIC   -----
        if (ShipManager.Instance.Reactor.Damage.DamageLevel > 0)
		{
            text.text = reactorDamaged.String;
            return;
        }
        if (!ShipManager.Instance.Reactor.IsTurnedOn)
		{
            text.text = reactorDisabled.String;
            return;
        }
        
        if ((ShipManager.Instance.OxygenLevel < 90 && ShipManager.Instance.oxygenDrain > 2) || (ShipManager.Instance.OxygenLevel < 50 && ShipManager.Instance.oxygenDrain > 0))  //maybe also check num damaged stations
		{
            text.text = generalRepair.String;
            return;
        }
        
        int count = 0;
        foreach (var obj in engines)
		{
            if (obj.CurrentFuel == 0)
			{
                count++;
            }
		}
        foreach (var obj in turrets)
		{
            if (obj.ShotsRemaining == 0)
			{
                count++;
            }
		}
        if (count > 1)
		{
            text.text = generalRefuel.String;
            return;
		}
        
        if (ShipManager.Instance.GetShipSpeed() == 0)
		{
            text.text = useEngines.String;
            return;
        }
        
        text.text = general.String;
	}

    public void OnEventChange(Level.Event.Type eventType)
	{
        lastEvent = currentEvent;
        currentEvent = eventType;

        eventActive = true;
        inEventLoop = false;

        //update text now
        timePassed = refreshRate;
	}


    private IEnumerator TextFlash()
    {
        Color initColor = text.color;
        Vector3 initScale = text.transform.localScale;

        Vector3 bigScale = Vector3.one * 2.5f;

        float t = 0;
        while (t < flashTime)
        {
            text.color = Color.Lerp(Color.red, initColor, t / flashTime);
            text.transform.localScale = Vector3.Lerp(bigScale, initScale, t / flashTime);

            t += Time.deltaTime;
            yield return null;
        }

        text.color = initColor;
        text.transform.localScale = initScale;
    }
}
