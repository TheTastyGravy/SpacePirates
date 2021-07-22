using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineStation : Interactable
{
    // The current power level of this engine. 0-2
    private int powerLevel = 0;
    public int PowerLevel { get => powerLevel; }



    public override void Activate(Interactor user)
    {
        // Increase power level, looping
        powerLevel++;
        if (powerLevel > 2)
            powerLevel = 0;
    }
}
