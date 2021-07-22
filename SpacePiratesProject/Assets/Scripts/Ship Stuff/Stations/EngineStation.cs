using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineStation : Interactable
{
    // The current power level of this engine. 0-2
    private int powerLevel = 0;
    public int PowerLevel { get => powerLevel; }

    public DamageStation damage;



	protected override void OnActivate(Interactor user)
    {
        // Increase power level, looping
        powerLevel++;

        bool isDamaged = damage != null;
        if (!isDamaged)
            isDamaged = damage.DamageLevel > 0;
        // If we are damaged, there are 2 power levels
        if (powerLevel > (isDamaged ? 1 : 2))
            powerLevel = 0;
    }
}
