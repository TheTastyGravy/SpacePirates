using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullHoleStation : Interactable
{
    public delegate void BasicDelegate();
    public BasicDelegate destroied;

    public GameObject shrapnelPrefab;
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

		CreateShrapnel();
	}

    protected override void OnInteract( Interactor user )
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

        CreateShrapnel();
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

    private void CreateShrapnel()
	{
        int count = Random.Range(damageLevels[size].minShrapnel, damageLevels[size].maxShrapnel+1);
        // Angle between shrapnel
        float angle = (maxAngle - minAngle) / count;

        for (int i = 0; i < count; i++)
		{
            // Get angle direction with random spread
            float lookAngle = minAngle + angle * 0.5f + angle * i;
            lookAngle += Random.Range(0, randomAngleVarience) - (randomAngleVarience * 0.5f);
            lookAngle *= Mathf.Deg2Rad;
            // Get angle as a direction
            Vector3 direction = Vector3.zero;
            direction.x = transform.forward.x * Mathf.Cos(lookAngle) - transform.forward.z * Mathf.Sin(lookAngle);
            direction.z = transform.forward.x * Mathf.Sin(lookAngle) + transform.forward.z * Mathf.Cos(lookAngle);
            // Create object and apply forward force
            GameObject shrapnel = Instantiate(shrapnelPrefab, transform.position + direction * startDist, transform.rotation);
            shrapnel.transform.forward = direction;
            shrapnel.GetComponent<Rigidbody>().AddForce(direction * velocity, ForceMode.VelocityChange);
        }
	}
}
