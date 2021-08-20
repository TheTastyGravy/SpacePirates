using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullHoleStation : Interactable
{
    [System.Serializable]
    public struct DamageLevel
	{
        public float oxygenLossRate;
        public int repairCount;
	}
    [Space]
    public DamageLevel[] damageLevels;

    [HideInInspector]
    public float oxygenLossRate;
    private int repairCount;

    private int size = 0;
    private int currentRepairCount = 0;

    [HideInInspector]
    public RoomManager room;
    [HideInInspector]
    public int holeIndex;



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
        room.RecalculateOxygenDrain();
    }
    private void DecreaseHoleSize()
	{
        size--;
        if (size < 0)
		{
            room.OnHoleDestroied(holeIndex);
            Destroy(gameObject);
        }
		else
		{
            oxygenLossRate = damageLevels[size].oxygenLossRate;
            repairCount = damageLevels[0].repairCount;
            currentRepairCount = 0;
            room.RecalculateOxygenDrain();
        }
	}

    protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
