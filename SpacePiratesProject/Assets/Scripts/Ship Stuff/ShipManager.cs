using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : Singleton<ShipManager>
{
    [Header("Oxygen")]
    public float maxOxygenLevel = 100;
    public float passiveOxygenLoss = 1;
    [Header("Avoidance")]
    [Range(0,1)]
    public float maxAvoidance = 0.25f;


    private ReactorStation[] reactors;
    private EngineStation[] engines;
    private RoomManager[] rooms;

    private float oxygenLevel;



    void Start()
    {
        reactors = GetComponentsInChildren<ReactorStation>();
        engines = GetComponentsInChildren<EngineStation>();
        rooms = GetComponentsInChildren<RoomManager>();

        oxygenLevel = maxOxygenLevel;
    }

    public void DamageShipAtPosition(Vector3 position)  //prob change from position to astroid script
    {
        HoleData holeData = new HoleData
        {
            hitPos = position,
            sqrDistance = Mathf.Infinity
        };

        // Find the closest hole positon, then damage the room
        foreach (var room in rooms)
        {
            room.FindClosestHolePos(ref holeData);
        }
        holeData.room.DamageRoom(holeData);
    }

    public float GetShipSpeed()
	{
        // Accumulate speed from engines
        float speed = 0;
        foreach (var engine in engines)
		{
            speed += engine.CurrentSpeed;
		}

        return speed;
	}

    public float GetShipAvoidance()
	{
        int active = 0;
        foreach (var obj in engines)
		{
            if (obj.IsTurnedOn)
                active++;
		}

        return maxAvoidance * (active / engines.Length);
	}


    void Update()
    {
        UpdateOxygen();
    }

    private void UpdateOxygen()
	{
        // Find the total loss rate
        float oxygenLossRate = passiveOxygenLoss;
        foreach (var room in rooms)
        {
            oxygenLossRate += room.localOxygenDrain;
        }
        // Decrease level by loss rate
        oxygenLevel -= oxygenLossRate * Time.deltaTime;

        // Find the regen rate and apply it
        float oxygenRegenRate = 0;
        foreach (var reactor in reactors)
		{
            oxygenRegenRate += reactor.CurrentOxygenRegen;
		}
        oxygenLevel += oxygenRegenRate * Time.deltaTime;
        if (oxygenLevel > maxOxygenLevel)
            oxygenLevel = maxOxygenLevel;


        if (oxygenLevel <= 0)
        {
            //game over
        }
    }
}
