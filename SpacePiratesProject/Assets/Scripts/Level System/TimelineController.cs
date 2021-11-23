using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class TimelineController : Singleton<TimelineController>
{
    [Tooltip("Prefabs that are used as icons for each event. Order is: astroid field, plasma storm")]
    public GameObject[] eventIconPrefabs;
    [Tooltip("Prefab that is used as the icon for the player ship")]
    public GameObject playerShipIconPrefab;
    [Tooltip("Prefab that is used for the ping effect")]
    public GameObject pingEffectPrefab;
    [Space]
    public float pingDistance = 40;
    public float pingTime = 1.5f;
    [Space]
    public float pingEffectFadeInTime = 0.2f;
    public float pingEffectFadeOutTime = 0.3f;
    public float flashFadeInTime = 0.2f;
    public float flashDisplayTime = 0.1f;
    public float flashFadeOutTime = 1;
    [Space]
    [Tooltip("The period between flashes. Set to 0 to disable")]
    public float playerShipFlashPeriod = 1.2f;
    [Space]
    public EventReference scanEvent;
    public EventReference pingEvent;

    private RectTransform timelineBase;
    private Material timelineMat;
    private Level level;
    // Inverse length of the level
    private float invLength;
    private float iconSize;
    private float pingSpeed;
    private class EventIcon
	{
		public RectTransform trans;
        public Image image;
        public float position;
        public Level.Event eventRef;
	}
    private List<EventIcon> icons = new List<EventIcon>();
    private Image playerShipIcon;
    private RectTransform playerShipRectTrans;
    private Image pingEffect;
    private RectTransform pingEffectRectTrans;
    private float playerPos;
	private float playerPosOffset = 0;
	private float shipPosOffset = 0;
    private Coroutine playerFlashRoutine;
    private bool updatePlayerPos = false;
	private float shipFlashTimePassed = 0;
    private float endlessPlayerShipFreezePos = 30;
    private float remainingLevelInvLength;
    private float remainingLevelScaler;
    // Data passed to timelineMat for highlighting
    private Vector4[] timelineHighlightData = new Vector4[2];

    // The distance between the actual player position and the saved offset
    private float PlayerPosDiference => (level.isEndless && playerPos > endlessPlayerShipFreezePos) ? endlessPlayerShipFreezePos : playerPos - playerPosOffset;
    // The position of the player ship icon along the timeline
    private float ShipIconPos => (timelineBase.rect.width - iconSize) * PlayerPosDiference * remainingLevelInvLength * remainingLevelScaler + shipPosOffset;


    // If something has broken in this script, abandon all hope


    public void Setup(Level level)
	{
        enabled = false;
        timelineBase = transform as RectTransform;
        this.level = level;
        invLength = 1.0f / level.length;
        remainingLevelInvLength = invLength;
        remainingLevelScaler = 1;
        iconSize = timelineBase.rect.height;
        pingSpeed = pingDistance / pingTime;
        updatePlayerPos = true;
        // Get the material on the timeline
        timelineMat = GetComponentInChildren<Image>().material;

        // Setup event icons
        for (int i = 0; i < level.events.Count; i++)
		{
            CreateEventIcon(level.events[i]);
        }

        // Setup player ship icon
        {
            // Create icon
            GameObject shipIconObj = Instantiate(playerShipIconPrefab, timelineBase);
            playerShipRectTrans = shipIconObj.transform as RectTransform;
            // Set hight to match the parent
            playerShipRectTrans.sizeDelta = new Vector2(0, iconSize);
            // Set position and size on X axis relitive to the left edge
            playerShipRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, iconSize);

            // Make image transparent if flashing
            playerShipIcon = shipIconObj.GetComponentInChildren<Image>();
            if (playerShipFlashPeriod > 0)
			{
                playerShipIcon.color = new Color(playerShipIcon.color.r, playerShipIcon.color.g, playerShipIcon.color.b, 0);
            }
        }

        // Setup ping effect
        {
            // Create object
            GameObject pingObject = Instantiate(pingEffectPrefab, timelineBase);
            pingEffectRectTrans = pingObject.transform as RectTransform;
            // Set hight to match the parent
            pingEffectRectTrans.sizeDelta = new Vector2(0, iconSize);
            // Set position and size on X axis relitive to the left edge
            pingEffectRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, iconSize);

            // Make image transparent
            pingEffect = pingObject.GetComponentInChildren<Image>();
            pingEffect.color = new Color(pingEffect.color.r, pingEffect.color.g, pingEffect.color.b, 0);
        }
    }

    private void CreateEventIcon(Level.Event _event)
    {
        // Check the event has an icon
        if ((int)_event.type >= eventIconPrefabs.Length)
            return;

        icons.Add(new EventIcon());
        int index = icons.Count - 1;
        // Set basic info
        icons[index].position = _event.start;
        icons[index].eventRef = _event;

        // Create icon for event
        GameObject iconObject = Instantiate(eventIconPrefabs[(int)_event.type], timelineBase);
        icons[index].trans = iconObject.transform as RectTransform;
        // Set hight to match the parent
        icons[index].trans.sizeDelta = new Vector2(0, iconSize);
        // Set position and size on X axis relitive to the left edge
        icons[index].trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (timelineBase.rect.width - iconSize) * icons[index].position * invLength + iconSize * 0.5f, iconSize);

        // Make image transparent
        icons[index].image = iconObject.GetComponentInChildren<Image>();
        Color color = icons[index].image.color;
        color.a = 0;
        icons[index].image.color = color;
    }
    
	void LateUpdate()
	{
        playerPos = LevelController.Instance.PlayerPosition;
        if (!updatePlayerPos)
            return;

        // If we are in an event, highlight the area it takes up
        bool inEvent = false;
        foreach (EventIcon icon in icons)
        {
            if (icon.eventRef.start < playerPos && playerPos < icon.eventRef.end)
            {
                inEvent = true;
                float start, end;
                if (level.isEndless)
                {
                    start = (icon.eventRef.start - playerPos + PlayerPosDiference) * invLength;
                    end = (icon.eventRef.end - playerPos + PlayerPosDiference) * invLength;
                }
                else
                {
                    // This doesnt account for the 'warping' that occurs when the levels length changes,
                    // and is slightly incorrect in that situation, but small enough to not be very noticable
                    start = icon.eventRef.start * invLength;
                    end = icon.eventRef.end * invLength;
                }

                // Set values. Use a minimum start value to prevent highlighting the very end of the
                // timeline in endless. The math is done to first put the values in range of where
                // icons go, then in range of magic numbers. I have no clue why they are nessesary.
                float widthScale = (timelineBase.rect.width - iconSize * 0.5f) / timelineBase.rect.width;
                timelineHighlightData[0].x = Mathf.Max(0.15f, (start * widthScale + (1 - widthScale)) * 0.772f + 0.12f);
                timelineHighlightData[0].y = (end * widthScale + (1 - widthScale)) * 0.772f + 0.12f;
                timelineHighlightData[0].z = (int)icon.eventRef.type;
                if (timelineHighlightData[0].w < 1)
                    timelineHighlightData[0].w += 1 * Time.deltaTime;
                break;
            }
        }
        // Fade highlight out when outside an event
        if (!inEvent && timelineHighlightData[0].w > 0)
        {
            timelineHighlightData[0].w -= 1 * Time.deltaTime;
        }
        // Pass the array to the material
        timelineMat.SetVectorArray("_HighlightData", timelineHighlightData);

        if (playerShipFlashPeriod > 0)
		{
			shipFlashTimePassed += Time.deltaTime;
            if (shipFlashTimePassed > playerShipFlashPeriod)
            {
				shipFlashTimePassed = 0;
                playerShipRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, ShipIconPos, iconSize);
                // Flash the ship
                if (playerFlashRoutine != null)
                {
                    StopCoroutine(playerFlashRoutine);
                }
                playerFlashRoutine = StartCoroutine(FlashIcon(playerShipIcon));
            }
        }
        else
		{
            playerShipRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, ShipIconPos, iconSize);
        }
    }

	// Called when the level has been changed
	public void UpdateTimeline()
	{
        // Update variable dependent on level length
        float shipIconPos = ShipIconPos;
        playerPosOffset = playerPos;
        invLength = 1.0f / level.length;
        if (!level.isEndless)
        {
            shipPosOffset = shipIconPos;
            remainingLevelInvLength = 1.0f / (level.length - playerPos);
            remainingLevelScaler = ((timelineBase.rect.width - iconSize) - shipIconPos) / (timelineBase.rect.width - iconSize);
        }

        // Update icons
        for (int i = 0; i < icons.Count; i++)
		{
			// If the event was removed, remove the icon
			if (icons[i].eventRef == null)
			{
				icons.RemoveAt(i);
				i--;
				continue;
			}
			// Update icon positions
			icons[i].position = icons[i].eventRef.start;
			icons[i].trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (timelineBase.rect.width - iconSize) * icons[i].position * invLength + iconSize * 0.5f, iconSize);
		}

        // Get all events we dont have an icon for
        List<Level.Event> events = level.events.FindAll(_event => !icons.Exists(icon => icon.eventRef == _event));
        foreach (Level.Event newEvent in events)
        {
            CreateEventIcon(newEvent);
        }
	}

	public void Ping()
	{
        StartCoroutine(PingTimeline());
    }

    private IEnumerator PingTimeline()
    {
        // Fade in the ping effect
        StartCoroutine(FadePingEffect(true));
        // Play sound effect
        if (!scanEvent.IsNull)
            RuntimeManager.PlayOneShot(scanEvent);

        // Find the first icon that will be pinged
        int index = icons.Count;
        for (int i = 0; i < icons.Count; i++)
        {
            if (icons[i].position > playerPos)
            {
                index = i;
                break;
            }
        }

        float time = 0;
        float startPos = PlayerPosDiference;
        float initPlayerPos = playerPos;
        float dist = 0;
        bool fadeOutFlag = false;
        while (time < pingTime)
		{
            // If the ping is ending, fade out the ping effect
            if (!fadeOutFlag && time >= pingTime - pingEffectFadeOutTime)
            {
                fadeOutFlag = true;
                StartCoroutine(FadePingEffect(false));
            }

            // Update ping effect position
            pingEffectRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 
                (timelineBase.rect.width - iconSize) * (startPos + dist) * remainingLevelInvLength * remainingLevelScaler + shipPosOffset, iconSize);

            // Flash icons as we pass them
            if (index < icons.Count && (initPlayerPos + dist) >= icons[index].position)
			{
                if (level.isEndless)
                    icons[index].trans.position = pingEffectRectTrans.position;
                StartCoroutine(FlashIcon(icons[index].image));
                // Play sound effect
                if (!pingEvent.IsNull)
                    RuntimeManager.PlayOneShot(pingEvent);
                index++;
			}
            // If we are within an event, highlight the area around us on the timeline
            if (index > 0 && icons[index - 1].eventRef.start < initPlayerPos + dist && initPlayerPos + dist < icons[index - 1].eventRef.end)
            {
                // Scale the ping icon position to the timeline, then convert into the range of magic numbers that make it work
                float value = ((pingEffectRectTrans.offsetMin.x + iconSize * 0.5f) / timelineBase.rect.width) * 0.772f + 0.12f;
                //[start, end, type, alpha]
                timelineHighlightData[1].x = value - 0.02f;
                timelineHighlightData[1].y = value + 0.02f;
                timelineHighlightData[1].z = (int)icons[index - 1].eventRef.type;
                timelineHighlightData[1].w = pingEffect.color.a;
            }
            else
            {
                // Not in an event, so reset the start and end to not draw a highlight
                timelineHighlightData[1].x = 0;
                timelineHighlightData[1].y = 0;
            }

            // Update time and distance
            time += Time.deltaTime;
            dist += pingSpeed * Time.deltaTime;
            // If we have reached the end of the level, fade out the ping effect and exit
            if ((level.isEndless ? startPos : initPlayerPos) + dist >= level.length)
            {
                if (!fadeOutFlag)
                    StartCoroutine(FadePingEffect(false));
                break;
            }
            yield return null;
		}
        // Remove the highlight
        timelineHighlightData[1].x = 0;
        timelineHighlightData[1].y = 0;
    }

    private IEnumerator FlashIcon(Image image)
	{
        // Fade in
        float time = 0;
        while (time < flashFadeInTime)
		{
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, time / flashFadeInTime));
            time += Time.deltaTime;
            yield return null;
		}

        // Display
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        yield return new WaitForSeconds(flashDisplayTime);

        // Fade out
        time = 0;
        while (time < flashFadeOutTime)
		{
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(1, 0, time / flashFadeOutTime));
            time += Time.deltaTime;
            yield return null;
		}

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        // If we are flashing the player, clear the routine ref
        if (image == playerShipIcon)
		{
            playerFlashRoutine = null;
        }
    }

    private IEnumerator FadePingEffect(bool fadeIn)
	{
        float start = fadeIn ? 0 : 1;
        float end = fadeIn ? 1 : 0;
        float totalTime = fadeIn ? pingEffectFadeInTime : pingEffectFadeOutTime;

        float time = 0;
        while (time < totalTime)
		{
            pingEffect.color = new Color(pingEffect.color.r, pingEffect.color.g, pingEffect.color.b, Mathf.Lerp(start, end, time / totalTime));
            time += Time.deltaTime;
            yield return null;
		}

        pingEffect.color = new Color(pingEffect.color.r, pingEffect.color.g, pingEffect.color.b, end);
    }
}
