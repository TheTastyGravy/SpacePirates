using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // The possible hole prefabs to create
    public GameObject[] holePrefabs;
    [Space]
    public float totalRoomOxygen = 100;
    public float baseOxygenRegenRate = 10;
    [Tooltip("When oxygen is below this level, players will start taking damage")]
    public float oxygenDamageLevel = 10;
    public float playerDamagePerSecond = 10;
    [Space]
    public float chanceToDamageStation = 0.1f;

	// All the positions a hole can be created at
	private Transform[] holePositions;
	private HullHoleStation[] holes;
    private float oxygenLevel = 100;
	public float OxygenLevel { get => oxygenLevel; }

    private DamageStation[] damageStations;

    private List< Character > players = new List< Character >();

    

    public void DamageRoom()
	{
        int posIndex = Random.Range(0, holePositions.Length);

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
        // Set up hole
        holes[index] = hole;
        hole.destroied = () => OnHoleDestroied(index);
    }
    // Called when a hole has been reparied
    private void OnHoleDestroied(int index)
	{
        holes[index] = null;
	}

	void Awake()
	{
		// Get hole positions
		List<Transform> transforms = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).CompareTag("HolePosition"))
			{
				transforms.Add(transform.GetChild(i));
			}
		}
		holePositions = transforms.ToArray();

		// There are the same number of posible holes as hole positions
		holes = new HullHoleStation[holePositions.Length];
		// Get all damage stations under this room
		damageStations = GetComponentsInChildren<DamageStation>();
	}

    void Update()
    {
        // Find the total loss rate
        float oxygenLossRate = 0;
        foreach (var hole in holes)
		{
            if (hole != null)
			{
                oxygenLossRate += hole.oxygenLossRate;
            }
		}
        // Decrease level by loss rate
        oxygenLevel -= oxygenLossRate * Time.deltaTime;
        if (oxygenLevel < 0)
            oxygenLevel = 0;
		// Only increate oxygen level if there is no loss
		if (oxygenLossRate == 0)
		{
			oxygenLevel += baseOxygenRegenRate * Time.deltaTime;
			if (oxygenLevel > totalRoomOxygen)
				oxygenLevel = totalRoomOxygen;
		}

        // If level is low enough, damage players in this room
        if (oxygenLevel < oxygenDamageLevel)
		{
            
        }
    }


	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
            players.Add(other.GetComponent<Character>());
		}
	}
	void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Player"))
        {
            players.Remove(other.GetComponent<Character>());
        }
    }
}
