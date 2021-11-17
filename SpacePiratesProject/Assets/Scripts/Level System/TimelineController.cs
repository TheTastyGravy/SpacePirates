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



	public void Setup(Level level)
	{
        enabled = false;
        timelineBase = transform as RectTransform;
        this.level = level;
        invLength = 1.0f / level.length;
        iconSize = timelineBase.rect.height;
        pingSpeed = pingDistance / pingTime;
        updatePlayerPos = true;

        // Setup event icons
        for (int i = 0; i < level.events.Count; i++)
		{
            Level.Event _event = level.events[i];
            // Check the event has an icon
            if ((int)_event.type >= eventIconPrefabs.Length)
                continue;

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
    
	void LateUpdate()
	{
        playerPos = LevelController.Instance.PlayerPosition;

        if (!updatePlayerPos)
            return;

        if (playerShipFlashPeriod > 0)
		{
			shipFlashTimePassed += Time.deltaTime;
            if (shipFlashTimePassed > playerShipFlashPeriod)
            {
				shipFlashTimePassed = 0;
                // Update player ship icon position
                playerShipRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (timelineBase.rect.width - iconSize) * (playerPos - playerPosOffset) * invLength + shipPosOffset, iconSize);
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
            // Update player ship icon position
            playerShipRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (timelineBase.rect.width - iconSize) * (playerPos - playerPosOffset) * invLength + shipPosOffset, iconSize);
        }
    }

	// Called when the level has been changed
	public void UpdateTimeline()
	{
		playerPosOffset = playerPos;
		shipPosOffset = (timelineBase.rect.width - iconSize) * playerPos * invLength;
		invLength = 1.0f / level.length;
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
        float dist = playerPos;
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
            pingEffectRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (timelineBase.rect.width - iconSize) * dist * invLength, iconSize);
            // Flash icons as we pass them
            if (index < icons.Count && dist >= icons[index].position)
			{
                StartCoroutine(FlashIcon(icons[index].image));
                // Play sound effect
                if (!pingEvent.IsNull)
                    RuntimeManager.PlayOneShot(pingEvent);
                index++;
			}

            // Update time and distance
            time += Time.deltaTime;
            dist += pingSpeed * Time.deltaTime;
            // If we have reached the end of the level, fade out the ping effect and exit
            if (dist >= level.length)
            {
                if (!fadeOutFlag)
                    StartCoroutine(FadePingEffect(false));
                break;
            }
            yield return null;
		}
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
