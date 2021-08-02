using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : Singleton<AIManager>
{
    public enum AIDifficulty
    { Easy, Medium, Hard }


    [HideInInspector]
    public List<Ship.Position> ships = new List<Ship.Position>();
    [HideInInspector]
    public List<float> engineEfficiencies = new List<float>();

    [HideInInspector]
    public int aiCount = 0;



    public void CreateAi(AIDifficulty difficulty)
	{
        aiCount++;


        ships.Add(new Ship.Position());

        float engine = 0;
        switch (difficulty)
		{
            case AIDifficulty.Easy:
                engine = 0.25f;
                break;
            case AIDifficulty.Medium:
                engine = 0.5f;
                break;
            case AIDifficulty.Hard:
                engine = 0.8f;
                break;
		}
        engineEfficiencies.Add(engine);
	}

    void Update()
    {
        
    }
}
