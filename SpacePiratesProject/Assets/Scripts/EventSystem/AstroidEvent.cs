using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidEvent : Event
{
	public GameObject astroidPrefab;

	private GameObject astroidInstance;
	private int targetRegion;
	private RoomManager targetRoom;

	public float time = 2;

	private float t = 0;
	private Vector3 startPos;



	public override bool CanBeActivated()
	{
		//if there is a free region, we can be activated
		foreach (var obj in EventManager.Instance.regions)
		{
			if (!obj)
			{
				return true;
			}
		}

		return false;
	}

	public override void Update()
	{
		t += Time.deltaTime;
		astroidInstance.transform.position = Vector3.Lerp(startPos, targetRoom.transform.position, t / time);

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
			targetRegion = Random.Range(0, 3);
			validTarget = !EventManager.Instance.regions[targetRegion];
		}
		//get random room from region
		targetRoom = HullManager.Instance.GetRandomRoom(targetRegion);
		//ocupy region
		EventManager.Instance.regions[targetRegion] = true;


		switch (targetRegion)
		{
			case 0:
				startPos = new Vector3(0, 0, 20);
				break;
			case 1:
				startPos = new Vector3(-20, 0, 0);
				break;
			case 2:
				startPos = new Vector3(20, 0, 0);
				break;
		}
		astroidInstance = Object.Instantiate(astroidPrefab, startPos, Quaternion.identity);
		astroidInstance.GetComponent<AstroidLogic>().onContact.AddListener(Stop);
	}

	protected override void OnStop()
	{
		Object.Destroy(astroidInstance);
		//free region
		EventManager.Instance.regions[targetRegion] = false;

		//apply damage
		targetRoom.DamageRoom();

		SoundController.Instance.Play("Impact", false);
	}
}
