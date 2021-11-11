 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class StatusManager : Singleton<StatusManager>
{
    [System.Serializable]
    public struct StringGroup
    {
        [SerializeField]
        private string[] strings;
        public EventReference dialogueEvent;

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
    public StringGroup asteroidInit;
    public StringGroup asteroidEnter;
    public StringGroup asteroidDurring;
    public StringGroup asteroidExit;
    public StringGroup asteroidPrestrike;
    [Space]
    public StringGroup stormInit;
    public StringGroup stormEnter;
    public StringGroup stormDurring;
    public StringGroup stormExit;
    public StringGroup stormPrestrike;
    [Space]
    public StringGroup shipInit;
    public StringGroup shipEnter;
    public StringGroup shipDurring;
    public StringGroup shipExit;

    private int currentMessageHash = -1;
    private Level.Event.Type currentEvent = Level.Event.Type.None;
    private bool eventActive = false;
    private EngineStation[] engines;
    private TurretStation[] turrets;
    private float timePassed = 0;
    private Coroutine textWriter;
    private Coroutine textFlasher;
    private Image cursorImage;
    private float cursorOffset;



    void Awake()
    {
        enabled = false;
        cursorImage = cursor.GetComponent<Image>();
        cursorOffset = (cursor.transform as RectTransform).rect.width * 0.5f + 5;
        cursorSpeed = 1 / cursorSpeed;

        ProcessStringGroup(gameStart);
        EventManager.Instance.OnEventChange += OnEventChange;
        Invoke(nameof(Init), 0.1f);

        Character.OnCheatActivated += () => SetText("CHEAT ACTIVATED", true);
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
        if (timePassed < refreshRate)
        {
            return;
        }

        //  -----   EVENTS   -----
        if (eventActive)
        {
            if (Random.Range(0f, 1f) < genericChance)
            {
                ProcessStringGroup(genericEvent);
            }
            else
            {
                switch (currentEvent)
                {
                    case Level.Event.Type.AstroidField:
                        ProcessStringGroup(asteroidDurring);
                        break;
                    case Level.Event.Type.PlasmaStorm:
                        ProcessStringGroup(stormDurring);
                        break;
                    case Level.Event.Type.ShipAttack:
                        ProcessStringGroup(shipDurring);
                        break;
                }
            }
            return;
        }

        
        //  -----   GENERIC   -----
        if (ShipManager.Instance.Reactor.Damage.DamageLevel > 0)
		{
            ProcessStringGroup(reactorDamaged);
            return;
        }
        if (!ShipManager.Instance.Reactor.IsTurnedOn)
		{
            ProcessStringGroup(reactorDisabled);
            return;
        }
        
        if ((ShipManager.Instance.OxygenLevel < 90 && ShipManager.Instance.oxygenDrain > 2) || (ShipManager.Instance.OxygenLevel < 50 && ShipManager.Instance.oxygenDrain > 0))  //maybe also check num damaged stations
		{
            ProcessStringGroup(generalRepair);
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
            ProcessStringGroup(generalRefuel);
            return;
		}
        
        if (ShipManager.Instance.GetShipSpeed() == 0)
		{
            ProcessStringGroup(useEngines);
            return;
        }

        ProcessStringGroup(general);
	}

    // Set text and play dialogue
    private void ProcessStringGroup(in StringGroup group, bool shouldFlash = false)
    {
        if (currentMessageHash == group.GetHashCode())
            return;

        currentMessageHash = group.GetHashCode();
        SetText(group.String, shouldFlash);
        if (!group.dialogueEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(group.dialogueEvent);
        }
    }

    public void OnEventChange(Level.Event.Type eventType, EventManager.EventStage stage)
	{
        currentEvent = eventType;
        eventActive = stage != EventManager.EventStage.END;

        if (stage == EventManager.EventStage.INIT)
        {
            switch (currentEvent)
            {
                case Level.Event.Type.AstroidField:
                    ProcessStringGroup(asteroidInit, true);
                    break;
                case Level.Event.Type.PlasmaStorm:
                    ProcessStringGroup(stormInit, true);
                    break;
                case Level.Event.Type.ShipAttack:
                    ProcessStringGroup(shipInit, true);
                    break;
            }
        }
        else if (stage == EventManager.EventStage.BEGIN)
        {
            switch (currentEvent)
            {
                case Level.Event.Type.AstroidField:
                    ProcessStringGroup(asteroidEnter);
                    break;
                case Level.Event.Type.PlasmaStorm:
                    ProcessStringGroup(stormEnter);
                    break;
                case Level.Event.Type.ShipAttack:
                    ProcessStringGroup(shipEnter);
                    break;
            }
        }
        else if (stage == EventManager.EventStage.END)
        {
            switch (currentEvent)
            {
                case Level.Event.Type.AstroidField:
                    ProcessStringGroup(asteroidExit);
                    break;
                case Level.Event.Type.PlasmaStorm:
                    ProcessStringGroup(stormExit);
                    break;
                case Level.Event.Type.ShipAttack:
                    ProcessStringGroup(shipExit);
                    break;
            }
        }
    }

    public void OnPrestrike(Level.Event.Type eventType)
    {
        if (eventType == Level.Event.Type.AstroidField)
        {
            ProcessStringGroup(asteroidPrestrike);
        }
        else if (eventType == Level.Event.Type.PlasmaStorm)
        {
            ProcessStringGroup(stormPrestrike);
        }
    }

    public void SetText(string message, bool shouldFlash = false)
	{
        if (text.text == message)
            return;

        timePassed = 0;
        // Set text and force the mesh to update with everything visible so we can get the hight
        text.SetText(message);
        text.maxVisibleCharacters = 9999;
        text.ForceMeshUpdate();
        float maxHeight = text.textBounds.size.y;

        if (textWriter != null)
            StopCoroutine(textWriter);
        textWriter = StartCoroutine(WriteText(maxHeight));

        if (shouldFlash)
        {
            if (textFlasher != null)
                StopCoroutine(textFlasher);
            textFlasher = StartCoroutine(TextFlash());
        }
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

            yield return new WaitForSecondsRealtime(cursorSpeed);
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

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        text.color = initColor;
        cursorImage.color = initColor;
        text.transform.localScale = initScale;
    }
}
