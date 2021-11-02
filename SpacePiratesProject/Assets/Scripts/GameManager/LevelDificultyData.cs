using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Difficulty Data", menuName = "Data/Level Diff Data")]
public class LevelDificultyData : ScriptableObject
{
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
        public float timeBetweenAsteroidWaves = 2;
        public int asteroidsPerWave = 4;
        [Space]
        public float timeBetweenStormDamage = 1;
        [Space]
        public int shipHealth = 10;
        public float shipFirePeriod = 2;
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

    
    public DiffSetting GetSetting(Difficulty diff, int playerCount)
	{
		return diff switch
		{
			Difficulty.Easy => easy,
			Difficulty.Medium => medium,
			Difficulty.Hard => hard,
			_ => null,
		};
	}
}
