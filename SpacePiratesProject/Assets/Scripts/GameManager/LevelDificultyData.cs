using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Difficulty Data", menuName = "Data/Level Diff Data")]
public class LevelDificultyData : ScriptableObject
{
    private static LevelDificultyData m_instance;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    [System.Serializable]
    public class DiffSetting
    {
        [Header("Level Generation")]
        public int minEventCount;
        public int maxEventCount;
        public float minEventLength;
        public float maxEventLength;
        public float extraLengthPerEvent = 5;
        [Header("Asteroids")]
        public float timeBetweenAstroids = 5;
        public float astroidSpawnDelay = 1.5f;
        [Header("Event Data")]
        [Tooltip("Time between the event being entered and properly starting")]
        public float initTime = 1f;
        [Space]
        public float timeBetweenAsteroidWaves = 2;
        public int asteroidsPerWave = 4;
        public float asteroidPrestrikeDelay = 0.25f;
        [Space]
        public float timeBetweenStormDamage = 1;
        public float plasmaStormPretrikeDelay = 1;
        [Space]
        public int shipHealth = 10;
        public float shipFirePeriod = 2;

        [Header("Engine")]
        public SettingEntry<float> maxFuel;
        public SettingEntry<float> startFuel;
        public SettingEntry<float> ammountOnRefuel;
        [Tooltip("The fuels rate of consumption")]
        public SettingEntry<float> fuelUsageRate;
        [Tooltip("The speed allied to the ship when turned on")]
        public SettingEntry<float> maxSpeed;
        [Header("Scanner")]
        public SettingEntry<float> interactionCooldown;
        [Header("Reactor")]
        public SettingEntry<float> baseOxygenRegenRate;
        [Tooltip("The ammount of time allowed between activating the switches")]
        public SettingEntry<float> timeBetweenSwitches;
        public SettingEntry<float> fuelGenerationTime;
        [Header("Turret")]
        public SettingEntry<int> maxShots;
        public SettingEntry<int> shotsPerFuel;
        public SettingEntry<int> startShots;
    }
    // A non-generic base class is nessesary for the property drawer to work
    public class BaseClass { }
    [System.Serializable]
    public class SettingEntry<T> : BaseClass
    {
        public T Value => playerCountValues[useSeperateValues ? (useMaxPlayers ? m_maxPlayerCount : m_playerCount) - 1 : 0];
        [SerializeField]
        private T[] playerCountValues;
        [SerializeField]
        private bool useSeperateValues = false;
        [SerializeField]
        private bool useMaxPlayers = false;
	}

    [Tooltip("How close to the ends of a level events can be placed")]
    public float edgeBoundry = 7.5f;
    [Tooltip("Basicly the minimum distance between events")]
    public float regionGap = 5;

    [SerializeField]
    private DiffSetting easy;
    [SerializeField]
    private DiffSetting medium;
    [SerializeField]
    private DiffSetting hard;
    [Header("Endless")]
    public float timeToDiffIncrease = 20;
    public float eventLengthAddition = 15;
    public float initTimeDecrease = 0;
    [Space]
    public float asteroidSpawnPeriodDecrease = 1;
    public float asteroidPromptDelayDecrease = 0.5f;
    [Space]
    public float timeBetweenAsteroidWavesDecrease = 0.25f;
    [Tooltip("Value used will be rounded down")]
    public float asteroidsPerWaveIncrease = 0.5f;
    public float asteroidPrestrikeDelayDecrease = 0;
    [Space]
    public float timeBetweenStormDamageDecrease = 0.1f;
    public float plasmaStormPretrikeDelayDecrease = 0;
    [Space]
    public int shipHealthIncrease = 1;
    public float shipFirePeriodIncrease = 0.2f;

    internal static int m_playerCount = 0;
    internal static int m_maxPlayerCount = 0;


    
    public DiffSetting GetSetting(Difficulty diff, int playerCount, int maxPlayerCount)
	{
        m_playerCount = playerCount;
        m_maxPlayerCount = maxPlayerCount;
        return diff switch
        {
            Difficulty.Easy => easy,
			Difficulty.Medium => medium,
			Difficulty.Hard => hard,
			_ => null,
		};
	}

    public static LevelDificultyData Instance
    {
        get
        {
            m_instance = m_instance ?? Resources.LoadAll<LevelDificultyData>("Levels")[0];
            return m_instance;
        }
    }
}
