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
    public GameObject cursor;
    [Tooltip("How long to wait between updating the text")]
    public float refreshRate = 1;
    [Tooltip("How long event start text is flashed for")]
    public float flashTime = 0.5f;
    [Tooltip("The speed text is writen out at")]
    public float cursorSpeed = 10;

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
    private Coroutine textWriter;
    private Image cursorImage;
    private float cursorOffset;



    void Awake()
    {
        enabled = false;
        cursorImage = cursor.GetComponent<Image>();
        cursorOffset = (cursor.transform as RectTransform).rect.width * 0.5f + 5;
        cursorSpeed = 1 / cursorSpeed;

        SetText(gameStart.String);
        EventManager.Instance.OnEventChange += OnEventChange;
        Invoke(nameof(Init), 0.1f);

        Character.OnCheatActivated += () => { SetText("CHEAT ACTIVATED"); timePassed = 0; StartCoroutine(TextFlash()); };
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
                            SetText(asteroidExit.String);
                            break;
                        case Level.Event.Type.PlasmaStorm:
                            SetText(stormExit.String);
                            break;
                        case Level.Event.Type.ShipAttack:
                            SetText(shipExit.String);
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
                            SetText(asteroidEnter.String);
                            break;
                        case Level.Event.Type.PlasmaStorm:
                            SetText(stormEnter.String);
                            break;
                        case Level.Event.Type.ShipAttack:
                            SetText(shipEnter.String);
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
                    SetText(genericEvent.String);
                }
                else
                {
                    switch (currentEvent)
                    {
                        case Level.Event.Type.AstroidField:
                            SetText(asteroidDurring.String);
                            break;
                        case Level.Event.Type.PlasmaStorm:
                            SetText(stormDurring.String);
                            break;
                        case Level.Event.Type.ShipAttack:
                            SetText(shipDurring.String);
                            break;
                    }
                }
            }

            return;
        }

        
        //  -----   GENERIC   -----
        if (ShipManager.Instance.Reactor.Damage.DamageLevel > 0)
		{
            SetText(reactorDamaged.String);
            return;
        }
        if (!ShipManager.Instance.Reactor.IsTurnedOn)
		{
            SetText(reactorDisabled.String);
            return;
        }
        
        if ((ShipManager.Instance.OxygenLevel < 90 && ShipManager.Instance.oxygenDrain > 2) || (ShipManager.Instance.OxygenLevel < 50 && ShipManager.Instance.oxygenDrain > 0))  //maybe also check num damaged stations
		{
            SetText(generalRepair.String);
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
            SetText(generalRefuel.String);
            return;
		}
        
        if (ShipManager.Instance.GetShipSpeed() == 0)
		{
            SetText(useEngines.String);
            return;
        }
        
        SetText(general.String);
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

    public void SetText(string message)
	{
        if (text.text == message)
            return;

        // Set text and force the mesh to update with everything visible so we can get the hight
        text.SetText(message);
        text.maxVisibleCharacters = 9999;
        text.ForceMeshUpdate();
        float maxHeight = text.textBounds.size.y;

        if (textWriter != null)
            StopCoroutine(textWriter);
        textWriter = StartCoroutine(WriteText(maxHeight));
    }
    
    private IEnumerator WriteText(float height)
	{
        // Used to get the position of characters
        TMP_TextInfo textInfo = text.GetTextInfo(text.text);

        text.maxVisibleCharacters = 0;
        while (text.maxVisibleCharacters < text.text.Length)
		{
            text.maxVisibleCharacters++;
            // Update cursor position
            Vector3 pos = textInfo.characterInfo[text.maxVisibleCharacters - 1].bottomRight;
            pos.x += cursorOffset;
            pos.y = -height;
            cursor.transform.localPosition = pos;

            yield return new WaitForSeconds(cursorSpeed);
        }
        textWriter = null;
    }

    private IEnumerator TextFlash()
    {
        Color initColor = text.color;
        Vector3 initScale = text.transform.localScale;

        Vector3 bigScale = Vector3.one * 2.5f;

        float t = 0;
        while (t < flashTime)
        {
            float value = t / flashTime;
            text.color = Color.Lerp(Color.red, initColor, value);
            cursorImage.color = text.color;
            text.transform.localScale = Vector3.Lerp(bigScale, initScale, value);

            t += Time.deltaTime;
            yield return null;
        }

        text.color = initColor;
        cursorImage.color = initColor;
        text.transform.localScale = initScale;
    }
}
