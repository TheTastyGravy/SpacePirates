using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullManager : Singleton<HullManager>
{
	public RoomManager[] leftRooms;
	public RoomManager[] rightRooms;
	public RoomManager[] frontRooms;



	public RoomManager GetRandomRoom(int region)//change from int to enum
	{
		if (region == 0)
		{
			int index = Random.Range(0, frontRooms.Length);
			return frontRooms[index];
		}
		else if (region == 1)
		{
			int index = Random.Range(0, leftRooms.Length);
			return leftRooms[index];
		}
		else
		{
			int index = Random.Range(0, rightRooms.Length);
			return rightRooms[index];
		}
	}


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
