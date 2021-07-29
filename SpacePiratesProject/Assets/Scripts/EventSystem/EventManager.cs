using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
	public float timeInterval = 1;
	[HideInInspector]
	public List<Event> activeEvents = new List<Event>();

	private float timePassed = 0;
	private PriorityQueue<Event> priorityQueue = new PriorityQueue<Event>();


	public enum Region
	{
		FRONT = 0,
		LEFT,
		RIGHT,
		COUNT
	}
	[HideInInspector]
	public bool[] regions = new bool[(int)Region.COUNT];

	//temp
	public GameObject astroidPrefab;




	void Update()
    {
		timePassed += Time.deltaTime;
		while (timePassed >= timeInterval)
		{
			timePassed -= timeInterval;
			CustomTick();
		}

		// Update active events
		for (int i = activeEvents.Count - 1; i >= 0; i--)
		{
			activeEvents[i].Update();
		}
	}

	public void AddEventToQueue(Event _event, int priority)
	{
		priorityQueue.Add(_event, priority);
	}

	private void CustomTick()
	{
		//temp
		AstroidEvent astroidEvent = new AstroidEvent();
		astroidEvent.astroidPrefab = astroidPrefab;
		AddEventToQueue(astroidEvent, 10);


		// Activate all events in the queue that can be activated
		while (!priorityQueue.IsEmpty())
		{
			Event _event = priorityQueue.Dequeue();
			if (_event.CanBeActivated())
			{
				_event.Start();
			}
		}
	}
}
