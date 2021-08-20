using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : Singleton<ShipManager>
{
    public float maxOxygenLevel = 100;
    public float passiveOxygenLoss = 1;

    private float oxygenLevel;

    //reactors
    private EngineStation[] engines;
    private RoomManager[] rooms;



    void Start()
    {
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

        //get regen rate from reactors
        float oxygenRegenRate = 0;

        oxygenLevel += oxygenRegenRate * Time.deltaTime;
        if (oxygenLevel > maxOxygenLevel)
            oxygenLevel = maxOxygenLevel;


        if (oxygenLevel <= 0)
        {
            //game over
        }
    }
}
