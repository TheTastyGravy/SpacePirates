using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // The possible hole prefabs to create
    public GameObject[] holePrefabs;
    public float chanceToDamageStation = 0.1f;

	// All the positions a hole can be created at
	private Transform[] holePositions;
	private HullHoleStation[] holes;
    private DamageStation[] damageStations;

	//private List<Character> players = new List<Character>();

	[HideInInspector]
	public float localOxygenDrain = 0;



	void Awake()
	{
		// Get hole positions using recursive search
		List<Transform> transforms = new List<Transform>();
		void Recursive(Transform trans)
		{
			for (int i = 0; i < trans.childCount; i++)
			{
				// If the child is a hole position, add it to the list, else go deeper
				Transform child = trans.GetChild(i);
				if (child.CompareTag("HolePosition"))
					transforms.Add(child);
				else if (child.childCount > 0)
					Recursive(child);
			}
		}
		Recursive(transform);
		holePositions = transforms.ToArray();

		// There are the same number of posible holes as hole positions
		holes = new HullHoleStation[holePositions.Length];
		// Get all damage stations under this room
		damageStations = GetComponentsInChildren<DamageStation>();
	}


	public void FindClosestHolePos(ref HoleData holeData)
	{
		for (int i = 0; i < holePositions.Length; i++)
		{
			// If this hole is closer than 
			float sqrDist = Vector3.SqrMagnitude(holePositions[i].position - holeData.hitPos);
			if (sqrDist < holeData.sqrDistance)
			{
				holeData.room = this;
				holeData.holeIndex = i;
				holeData.sqrDistance = sqrDist;
			}
		}
	}

	public void DamageRoom(in HoleData holeData)
	{
        int posIndex = holeData.holeIndex;

        // If the hole already exists make it bigger, otherwise make a new hole
        if (holes[posIndex] != null)
		{
            holes[posIndex].IncreaseHoleSize();
        }
		else
		{
            CreateHole(posIndex);
        }

        // Random chance to damage each station
        foreach (var obj in damageStations)
		{
            if (Random.Range(0f, 1f) < chanceToDamageStation)
			{
                obj.Damage();
			}
		}
	}

    private void CreateHole(int index)
	{
        GameObject prefab = holePrefabs[Random.Range(0, holePrefabs.Length)];

        // Create hole at position and get station script
        HullHoleStation hole = Instantiate(prefab, holePositions[index].position, holePositions[index].rotation, transform).GetComponent<HullHoleStation>();
        holes[index] = hole;
		hole.room = this;
		hole.holeIndex = index;

		Invoke("RecalculateOxygenDrain", 0);
	}
    // Called when a hole has been reparied
    internal void OnHoleDestroied(int index)
	{
        holes[index] = null;
		RecalculateOxygenDrain();
	}

	internal void RecalculateOxygenDrain()
	{
		localOxygenDrain = 0;

		foreach (var hole in holes)
		{
			if (hole != null)
			{
				localOxygenDrain += hole.oxygenLossRate;
			}
		}
	}


	//void OnTriggerEnter(Collider other)
	//{
	//	if (other.CompareTag("Player"))
	//	{
    //        players.Add(other.GetComponent<Character>());
	//	}
	//}
	//void OnTriggerExit(Collider other)
	//{
    //    if (other.CompareTag("Player"))
    //    {
    //        players.Remove(other.GetComponent<Character>());
    //    }
    //}
}

public struct HoleData
{
	// The position the ship has been hit
	public Vector3 hitPos;
	// The distance from hitPos to the closest hole pos
	public float sqrDistance;
	// The room the closest hole pos belongs to
	public RoomManager room;
	// The hole index of the closest hole
	public int holeIndex;
}