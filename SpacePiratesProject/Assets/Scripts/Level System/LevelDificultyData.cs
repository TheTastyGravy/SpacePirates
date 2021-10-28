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
        public int minEventCount;
        public int maxEventCount;
        public float minEventLength;
        public float maxEventLength;

        public float extraLengthPerEvent = 5;
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

    
    public DiffSetting GetSetting(Difficulty diff)
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
