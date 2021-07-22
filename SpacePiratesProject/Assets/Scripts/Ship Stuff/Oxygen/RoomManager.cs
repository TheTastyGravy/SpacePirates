using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // All the positions a hole can be created at
    public Transform[] holePositions;
    // The possible hole prefabs to create
    public GameObject[] holePrefabs;
    [Space]
    public float totalRoomOxygen = 100;
    public float baseOxygenRegenRate = 10;
    [Tooltip("When oxygen is below this level, players will start taking damage")]
    public float oxygenDamageLevel = 10;
    public float playerDamagePerSecond = 10;


    private HullHoleStation[] holes;
    private float oxygenLevel = 100;

    private List<PlayerHealth> players = new List<PlayerHealth>();

    

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


    void Start()
    {
        // There can be the same number of holes as hole positions
        holes = new HullHoleStation[holePositions.Length];
    }

    void Update()
    {
        // Increase level by base amount
        oxygenLevel += baseOxygenRegenRate * Time.deltaTime;
        if (oxygenLevel > totalRoomOxygen)
            oxygenLevel = totalRoomOxygen;

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


        // If level is low enough, damage players in this room
        if (oxygenLevel < oxygenDamageLevel)
		{
            foreach (var player in players)
			{
                player.UpdateHealth(-playerDamagePerSecond * Time.deltaTime);
			}
        }
    }


	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
            players.Add(other.GetComponent<PlayerHealth>());
		}
	}
	void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Player"))
        {
            players.Remove(other.GetComponent<PlayerHealth>());
        }
    }
}
