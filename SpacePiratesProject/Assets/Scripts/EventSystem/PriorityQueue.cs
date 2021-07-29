using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
	struct Entry
	{
		public T data;
		public int priority;
		public float time;
	}

	private List<Entry> queue = new List<Entry>();



	public void Add(in T data, int priority)
	{
		Entry entry;
		entry.data = data;
		entry.priority = priority;
		entry.time = Time.time;

		// Find where to insert the entry
		for (int i = 0; i < queue.Count; i++)
		{
			if (Compare(entry, queue[i]))
			{
				queue.Insert(i, entry);
				return;
			}
		}
		//add to end
		queue.Add(entry);
	}

	private bool Compare(in Entry e1, in Entry e2)
	{
		if (e1.priority > e2.priority)
			return true;
		if (e1.priority < e2.priority)
			return false;

		// The priority is the same, so compare time
		return e1.time <= e2.time;
	}

	public T Dequeue()
	{
		T data = queue[0].data;
		queue.RemoveAt(0);
		return data;
	}

	public bool IsEmpty()
	{
		return queue.Count == 0;
	}
}
