using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewLevel", menuName = "", order = 1)]
public class Level : ScriptableObject
{
    [Serializable]
    public class Event
    {
        public float start, end;
        public Type type;

        public enum Type
        {
            AstroidField,
            PlasmaStorm,
            ShipAttack,
            None
        }
    }

    public LevelDificultyData diffData;

    [Tooltip("The difficulty preset used when generating a level")]
    public LevelDificultyData.Difficulty difficulty;
    [Tooltip("Generate a random level when the game starts")]
    public bool useRandomEvents = false;
    [Tooltip("Generate a random level now")]
    public bool generateRandomLevelNow = false;
    [Space]
    [Tooltip("This value is generated with the level")]
    public float length;
    public Event[] events;



    // Called by LevelController.Start()
    public void Setup()
	{
        if (useRandomEvents)
		{
            if (diffData == null)
            {
                Debug.LogError("Level needs a LevelDificultyData assigned to generate random levels");
                return;
            }

            GenerateRandomLevel();
		}
    }

    // Called when a value is changed in the editor
	void OnValidate()
	{
        if (generateRandomLevelNow)
		{
            generateRandomLevelNow = false;
            if (diffData == null)
            {
                Debug.LogError("Level needs a LevelDificultyData assigned to generate random levels");
                return;
            }

            GenerateRandomLevel();
		}
	}

    private void GenerateRandomLevel()
	{
        LevelDificultyData.DiffSetting settings = diffData.GetSetting(difficulty);

        int eventCount = UnityEngine.Random.Range(settings.minEventCount, settings.maxEventCount + 1);
        // Determine length of level
        length = diffData.edgeBoundry * 2 + diffData.regionGap * (eventCount + 1) + (settings.maxEventLength + settings.extraLengthPerEvent) * eventCount;

        events = new Event[eventCount];
        // The area an event can be created within
        float eventArea = (length - diffData.edgeBoundry * 2) / eventCount;

        for (int i = 0; i < eventCount; i++)
        {
            float halfLength = UnityEngine.Random.Range(settings.minEventLength, settings.maxEventLength) * 0.5f;
            float pos = UnityEngine.Random.Range(halfLength + diffData.regionGap * 0.5f, eventArea - halfLength - diffData.regionGap * 0.5f) + diffData.edgeBoundry + eventArea * i;

            float start = pos - halfLength;
            float end = pos + halfLength;
            // Prevent events from overlapping or going outside the boundry
            start = Mathf.Max(start, i > 0 ? events[i - 1].end : diffData.edgeBoundry);
            if (i == eventCount - 1)
			{
                end = Mathf.Min(end, length - diffData.edgeBoundry);
			}

            events[i] = new Event()
            {
                start = start,
                end = end,
                // Get random int in range of enum count, and cast to Event.Type
                type = (Event.Type)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Event.Type)).Length - 1)
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