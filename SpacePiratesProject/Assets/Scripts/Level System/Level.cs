using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewLevel", menuName = "", order = 1)]
public class Level : ScriptableObject
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    [Serializable]
    public class Event
    {
        public float start, end;
        public Type type;

        public enum Type
        {
            AstroidField,
            PlasmaStorm,
            ShipAttack
        }
    }

    [Tooltip("The difficulty preset used when generating a level")]
    public Difficulty difficulty;
    [Tooltip("Generate a random level when the game starts")]
    public bool useRandomEvents = false;
    [Tooltip("Generate a random level now")]
    public bool generateRandomLevelNow = false;
    [Space]
    public float length = 100;
    public Event[] events;

    // How close to the ends of a level events can be placed
    private static float edgeboundry = 7.5f;
    private class DiffSetting
    {
        public int minEventCount;
        public int maxEventCount;
        public float minEventLength;
        public float maxEventLength;
    }
    private static DiffSetting[] diffSettings = new DiffSetting[]
    {
        // EASY
        new DiffSetting
        { 
            minEventCount = 2,
            maxEventCount = 2,
            minEventLength = 5,
            maxEventLength = 10
        },
        // MEDIUM
        new DiffSetting
        {
            minEventCount = 3,
            maxEventCount = 4,
            minEventLength = 7.5f,
            maxEventLength = 15
        },
        // HARD
        new DiffSetting
        {
            minEventCount = 4,
            maxEventCount = 6,
            minEventLength = 10,
            maxEventLength = 20
        }
    };



    // Called by LevelController.Start()
	public void Setup()
	{
        if (useRandomEvents)
		{
            GenerateRandomLevel();
		}
    }

    // Called when a value is changed in the editor
	void OnValidate()
	{
        if (generateRandomLevelNow)
		{
            generateRandomLevelNow = false;
            GenerateRandomLevel();
		}
	}

    private void GenerateRandomLevel()
	{
        DiffSetting settings = diffSettings[(int)difficulty];

        int eventCount = UnityEngine.Random.Range(settings.minEventCount, settings.maxEventCount + 1);
        events = new Event[eventCount];
        // The area an event can be created within
        float eventArea = (length - edgeboundry * 2) / eventCount;

        for (int i = 0; i < eventCount; i++)
        {
            float halfLength = UnityEngine.Random.Range(settings.minEventLength, settings.maxEventLength) * 0.5f;
            float pos = UnityEngine.Random.Range(halfLength, eventArea - halfLength) + edgeboundry + eventArea * i;

            float start = pos - halfLength;
            float end = pos + halfLength;
            // Prevent events from overlapping or going outside the boundry
            start = Mathf.Max(start, i > 0 ? events[i - 1].end : edgeboundry);
            if (i == eventCount - 1)
			{
                end = Mathf.Min(end, length - edgeboundry);
			}

            events[i] = new Event()
            {
                start = start,
                end = end,
                // Get random int in range of enum count, and cast to Event.Type
                type = (Event.Type)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Event.Type)).Length)
            };
        }
    }

	public static Level[] All
    {
        get
        {
            m_Levels = m_Levels ?? Resources.LoadAll< Level >( "Levels" );
            return m_Levels;
        }
    }

    public Event this[ int a_Index ] => events[ a_Index ];

    public static Level GetLevel( int a_Index )
    {
        return a_Index < 0 || a_Index >= All.Length ? null : All[ a_Index ];
    }

    private static Level[] m_Levels;
}