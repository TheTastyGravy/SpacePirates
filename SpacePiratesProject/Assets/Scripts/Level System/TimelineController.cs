using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineController : Singleton<TimelineController>
{
    [Tooltip("Prefabs that are used as icons for each event. Order is: astroid field, plasma storm")]
    public GameObject[] eventIconPrefabs;
    [Tooltip("Prefab that is used as the icon for the player ship")]
    public GameObject playerShipIconPrefab;


    private RectTransform timelineBase;
    private Level level;
    // Inverse length of the level
    private float invLength;

    private struct EventIcon
	{
        public Image image;
        public float position;
        public Level.Event.Type eventType;
	}
    private EventIcon[] icons;
    private Image playerShipIcon;



    public void Setup(Level level)
	{
        timelineBase = transform as RectTransform;
        this.level = level;
        invLength = 1.0f / level.length;

        // Setup event icons
        icons = new EventIcon[level.events.Length];
        for (int i = 0; i < icons.Length; i++)
		{
            Level.Event _event = level.events[i];
            // Set basic info
            icons[i].position = (_event.start + _event.end) * 0.5f;
            icons[i].eventType = _event.type;

            // Create icon for event
            GameObject iconObject = Instantiate(eventIconPrefabs[(int)_event.type], timelineBase);
            RectTransform rectTrans = iconObject.transform as RectTransform;
            // Set hight to match the parent
            rectTrans.sizeDelta = new Vector2(0, timelineBase.rect.height);
            // Set position and size on X axis relitive to the left edge
            rectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, timelineBase.rect.width * icons[i].position * invLength, timelineBase.rect.height);

            // Make image transparent
            icons[i].image = iconObject.GetComponent<Image>();
            icons[i].image.color = Color.clear;
        }

        // Setup player ship icon
        GameObject shipIconObj = Instantiate(playerShipIconPrefab, timelineBase);
        RectTransform rect = shipIconObj.transform as RectTransform;
        // Set hight to match the parent
        rect.sizeDelta = new Vector2(0, timelineBase.rect.height);
        // Set position and size on X axis relitive to the left edge
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, timelineBase.rect.height);

        // Make image transparent
        playerShipIcon = shipIconObj.GetComponent<Image>();
        playerShipIcon.color = Color.clear;
    }

    void Update()
    {
        
    }


    public void UpdateTimeline()
	{



	}
}
