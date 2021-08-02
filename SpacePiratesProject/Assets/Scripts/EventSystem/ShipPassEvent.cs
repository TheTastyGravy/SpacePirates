using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPassEvent : Event
{
	public GameObject shipPrefab;
	// Is the enemy ship passing the player?
	public bool isPassing = false;

	private GameObject shipInstance;

	private float time = 1;
	private float t = 0;

	private Vector3 startPos, endPos;
	private float lastZPos;

	private int targetRegion;
	private RoomManager targetRoom;



	public override bool CanBeActivated()
	{
		if (!EventManager.Instance.regions[(int)EventManager.Region.LEFT])
		{
			return true;
		}
		if (!EventManager.Instance.regions[(int)EventManager.Region.RIGHT])
		{
			return true;
		}

		return false;
	}

	public override void Update()
	{
		t += Time.deltaTime;
		shipInstance.transform.position = Vector3.Lerp(startPos, endPos, t / time);

		//if the last x was on the other side of the room last update, we have just passed the player
		if ((lastZPos < targetRoom.transform.position.z) !=
			(shipInstance.transform.position.z < targetRoom.transform.position.z))
		{
			targetRoom.DamageRoom();
			SoundController.Instance.Play("ShipPassing", false);
		}
		lastZPos = shipInstance.transform.position.z;
		
		if (t >= time)
		{
			Stop();
		}
	}

	protected override void OnStart()
	{
		//get a target region
		bool validTarget = false;
		while (!validTarget)
		{
			targetRegion = Random.Range(1, 3);
			validTarget = !EventManager.Instance.regions[targetRegion];
		}
		//get random room from region to attack
		targetRoom = HullManager.Instance.GetRandomRoom(targetRegion);
		//ocupy region
		EventManager.Instance.regions[targetRegion] = true;

		//determine start and end position
		startPos = new Vector3(-40, 0, 43);
		if (targetRegion == 2)
			startPos.x = -startPos.x;
		endPos = startPos;
		endPos.z = -50;
		//flip if passing the player
		if (isPassing)
		{
			Vector3 temp = startPos;
			startPos = endPos;
			endPos = temp;
		}

		shipInstance = Object.Instantiate(shipPrefab, startPos, Quaternion.identity);
		lastZPos = startPos.z;
	}

	protected override void OnStop()
	{
		Object.Destroy(shipInstance);
		//free region
		EventManager.Instance.regions[targetRegion] = false;
	}
}
