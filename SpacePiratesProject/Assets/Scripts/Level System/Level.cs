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
    public List<Event> events = new List<Event>();
	[Space]
	public bool isEndless;
	public int endlessEventCount = 2;

	// Values used for event creation
	[HideInInspector]
	public float eventArea;


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
		events.Clear();
		// Player count doesnt matter for levels
		LevelDificultyData.DiffSetting settings = diffData.GetSetting(difficulty, 2, 2);

		eventArea = diffData.regionGap + settings.maxEventLength + settings.extraLengthPerEvent;
		int eventCount = isEndless ? endlessEventCount : UnityEngine.Random.Range(settings.minEventCount, settings.maxEventCount + 1);
        length = diffData.edgeBoundry * 2 + eventArea * eventCount;
		// Create events
		for (int i = 0; i < eventCount; i++)
        {
			CreateEvent(UnityEngine.Random.Range(settings.minEventLength, settings.maxEventLength));
		}
    }

	internal void CreateEvent(float eventLength, bool removeFirstEvent = false)
	{
		int index = events.Count;

		// Find the center position to determine the start and end around it
		float offset = diffData.edgeBoundry;
		if (index > 0)
		{
            // Round the previous events end pos up to a multiple of eventArea. This is done instead of 
            // using eventArea * index to make it independent of event index, being important for endless mode
            //offset += Mathf.Round((events[index - 1].end - offset - 1) / eventArea) * eventArea;
            offset += events[index - 1].end;
		}
        float halfLength = eventLength * 0.5f;
        float pos = UnityEngine.Random.Range(halfLength + diffData.regionGap * 0.5f, eventArea - halfLength - diffData.regionGap * 0.5f) + offset;
		events.Add(new Event()
		{
			start = pos - halfLength,
			end = pos + halfLength,
			// Get random int in range of enum count, and cast to Event.Type
			type = (Event.Type)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Event.Type)).Length - 1)
		});
		
		// If this is the same type as the last event, change it
		if (index > 0 && events[index].type == events[index - 1].type)
		{
			switch (events[index].type)
			{
				case Event.Type.AstroidField:
					events[index].type = Event.Type.PlasmaStorm;
					break;
				case Event.Type.PlasmaStorm:
					events[index].type = Event.Type.ShipAttack;
					break;
				case Event.Type.ShipAttack:
					events[index].type = Event.Type.AstroidField;
					break;
			}
		}

		if (removeFirstEvent)
		{
			events.RemoveAt(0);
		}
	}

	public void ResizeEvent(int index, float newEnd)
	{
		float diff = events[index].end - newEnd;
		events[index].end = newEnd;
		// Move all following events forward to avoid a larger gap
		for (int i = index + 1; i < events.Count; i++)
		{
			events[i].start -= diff;
			events[i].end -= diff;
		}
		if (!isEndless)
			length -= diff;
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