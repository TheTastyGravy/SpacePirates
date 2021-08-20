using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LevelController : Singleton<LevelController>
{
    public Ship.Position PlayerShipPosition => playerShip;
    public Track track;
    public Ship ship;

	[Space]
	//temp for event
	public GameObject shipPrefab;

	private Ship.Position playerShip;
    private AIManager ai;

	

	void Start()
    {
        track = Track.GetTrack( GameManager.SelectedTrack );
        ship = Ship.GetShip( GameManager.SelectedShip );
        Instantiate( ship.ShipPrefab );
        HUDController.Instance.ManeuverDisplay.UpdateCards();
		ai = AIManager.Instance;
		ai.CreateAi(AIManager.AIDifficulty.Easy);
		ai.CreateAi(AIManager.AIDifficulty.Medium);
		ai.CreateAi(AIManager.AIDifficulty.Hard);
	}

    void Update()
    {
		// Get engine efficiency for player
		float playerEngine = ShipManager.Instance.GetShipSpeed();

		// Get new ship positions
		Ship.Position newPlayerShip = GetNewShipPos(playerShip, playerEngine);
        
        int timeRemaining = GetSecondsRemaining( playerEngine );
        HUDController.Instance.ManeuverDisplay.UpdateETADisplay( timeRemaining );

        // Track change, push to ManeuverDisplay
        if ( newPlayerShip.TrackSegment > playerShip.TrackSegment )
        {
            HUDController.Instance.ManeuverDisplay.TriggerSlide();
        }

        List<Ship.Position> newAiShips = new List<Ship.Position>();
        for (int i = 0; i < ai.aiCount; i++)
		{
            newAiShips.Add(GetNewShipPos(ai.ships[i], ai.engineEfficiencies[i]));
        }

        // Check if the player has any passes
        CheckForPasses(newPlayerShip, newAiShips);

		// Play alert if the player in on a new segment
		if (newPlayerShip.TrackSegment != playerShip.TrackSegment)
		{
			SoundManager.Instance.Play("TrackUpdates", false);
		}

        // Update positions
        playerShip = newPlayerShip;
        ai.ships = newAiShips;
        // Check if the player has reached the end of the track
        if (playerShip.TrackSegment == track.Length-1 && playerShip.SegmentPosition == 1)
		{
            GameManager.CurrentState = GameManager.GameState.SUMMARY;
        }
    }

    private int GetSecondsRemaining( float a_EngineEfficiency )
    {
        return ( int )( ( 1.0f - playerShip.SegmentPosition ) * track[ playerShip.TrackSegment ].TimeToComplete / a_EngineEfficiency ) + 1;
    }

    private Ship.Position GetNewShipPos(Ship.Position shipPos, float engineEfficiency)
	{
        // Update pos, looping segment dist into track index
        shipPos.SegmentPosition += engineEfficiency * Time.deltaTime / track[ shipPos.TrackSegment ].TimeToComplete;
        if (shipPos.SegmentPosition > 1)
		{
            shipPos.SegmentPosition -= 1;
            shipPos.TrackSegment++;
		}
        // Prevent ship position from going past the end of the track
        if (shipPos.TrackSegment >= track.Length)
		{
            shipPos.SegmentPosition = 1;
            shipPos.TrackSegment = track.Length - 1;
        }

        return shipPos;
	}

    private void CheckForPasses(in Ship.Position newPlayerShip, in List<Ship.Position> newAiShips)
	{
        for (int i = 0; i < ai.aiCount; i++)
		{
            //determine last state
            bool wasBehind = true;
            if (playerShip.TrackSegment < ai.ships[i].TrackSegment)
			{
                wasBehind = false;
			}
            else if (playerShip.TrackSegment == ai.ships[i].TrackSegment)
			{
                wasBehind = playerShip.SegmentPosition > ai.ships[i].SegmentPosition;
			}
            //determine current state
            bool isBehind = true;
            if (newPlayerShip.TrackSegment < newAiShips[i].TrackSegment)
			{
                isBehind = false;
			}
            else if (newPlayerShip.TrackSegment == newAiShips[i].TrackSegment)
			{
                isBehind = newPlayerShip.SegmentPosition > newAiShips[i].SegmentPosition;
			}


            if (isBehind != wasBehind)
			{
				ShipPassEvent _event = new ShipPassEvent();
				_event.shipPrefab = shipPrefab;
				_event.isPassing = !isBehind;
				EventManager.Instance.AddEventToQueue(_event, 100);
			}
		}
	}
}
