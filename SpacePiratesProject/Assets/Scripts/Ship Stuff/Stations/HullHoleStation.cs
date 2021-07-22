using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullHoleStation : Interactable
{
    public delegate void BasicDelegate();
    public BasicDelegate destroied;

    [System.Serializable]
    public struct DamageLevel
	{
        public float oxygenLossRate;
        public int repairCount;
	}
    public DamageLevel[] damageLevels;

    [HideInInspector]
    public float oxygenLossRate;
    private int repairCount;

    private int size = 0;
    private int currentRepairCount = 0;



    void Start()
    {
        oxygenLossRate = damageLevels[0].oxygenLossRate;
        repairCount = damageLevels[0].repairCount;
    }

    protected override void OnActivate(Interactor user)
	{
        currentRepairCount++;

        // When fully repaired, reduce the hole size
        if (currentRepairCount >= repairCount)
		{
            DecreaseHoleSize();
		}
	}

    public void IncreaseHoleSize()
	{
        size++;
        if (size >= damageLevels.Length)
            size = damageLevels.Length - 1;

        oxygenLossRate = damageLevels[size].oxygenLossRate;
        repairCount = damageLevels[0].repairCount;
        currentRepairCount = 0;
    }
    private void DecreaseHoleSize()
	{
        size--;
        if (size < 0)
		{
            destroied();
            Destroy(gameObject);
        }
		else
		{
            oxygenLossRate = damageLevels[size].oxygenLossRate;
            repairCount = damageLevels[0].repairCount;
            currentRepairCount = 0;
        }
	}
}
