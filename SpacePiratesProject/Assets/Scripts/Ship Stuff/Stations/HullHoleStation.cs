using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullHoleStation : Interactable
{
    public delegate void BasicDelegate();
    public BasicDelegate destroied;

    public float minAngle, maxAngle;
    public float randomAngleVarience = 10;
    public float startDist = 0.5f;
    public float velocity = 5;

    [System.Serializable]
    public struct DamageLevel
	{
        public float oxygenLossRate;
        public int repairCount;

        public int minShrapnel, maxShrapnel;
	}
    [Space]
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

    protected override void OnInteractStart(Interactor interactor)
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

    protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
