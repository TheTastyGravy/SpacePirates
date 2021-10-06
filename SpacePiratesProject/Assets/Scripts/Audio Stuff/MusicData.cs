using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Music Data", menuName = "Data/Music Data")]
public class MusicData : ScriptableObject
{
    public List<MusicInfo> musicData;
    // Used to create a new MusicInfo instance
    [SerializeField]
    private bool addFlag;
    [SerializeField]
    private bool printFlag;

    [System.Serializable]
    public class MusicInfo
    {
        // Name displayed in the inspector
        [SerializeField]
        internal string name = "New Element";

        // Flag enum, contains multiple states
        public GameManager.GameState scenes;
        public FMODUnity.EventReference musicEvent;
        public float fadeTime = 0;

        // Used to delete the object
        [SerializeField]
        internal bool removeFlag = false;
    }



	public MusicInfo GetInfo(GameManager.GameState state)
	{
        if (state == GameManager.GameState.NONE)
            return null;

        foreach (var obj in musicData)
		{
            GameManager.GameState flags = obj.scenes;
            if (flags.HasFlag(state))
			{
                return obj;
			}
		}

        return null;
	}

    // Print the music info name that will be used for each game state
    private void PrintResults()
	{
        string message = "";
        int count = 0;
        foreach (GameManager.GameState obj in System.Enum.GetValues(typeof(GameManager.GameState)))
        {
            if (obj == GameManager.GameState.NONE)
                continue;

            MusicInfo info = GetInfo(obj);
            message += string.Format("<size=12>{0}{1,-11}\t{2}</size>\n", count == 0 ? "" : "\t    ", obj+":", info == null ? "null" : info.name);
            count++;
            if (count == 2)
			{
                Debug.Log(message);
                message = "";
                count = 0;
			}
        }
        if (message.Length > 0)
            Debug.Log(message);
    }

    void OnValidate()
    {
        for (int i = 0; i < musicData.Count; i++)
        {
            if (musicData[i].removeFlag)
            {
                musicData.RemoveAt(i);
                i--;
            }
        }

        if (addFlag)
		{
            addFlag = false;
            musicData.Add(new MusicInfo());
		}

        if (printFlag)
		{
            printFlag = false;
            PrintResults();
        }
    }
}
