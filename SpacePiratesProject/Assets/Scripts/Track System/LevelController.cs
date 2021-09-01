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


	private Ship.Position playerShip;
    private AIManager ai;

	

	void Start()
    {
        track = Track.GetTrack( GameManager.SelectedTrack );
        ship = Ship.GetShip( GameManager.SelectedShip );
        Instantiate( ship.ShipPrefab );
        //HUDController.Instance.ManeuverDisplay.UpdateCards();
		ai = AIManager.Instance;
	}

    void Update()
    {
		// Get engine efficiency for player
		float playerEngine = ShipManager.Instance.GetShipSpeed();

		// Get new ship positions
		Ship.Position newPlayerShip = GetNewShipPos(playerShip, playerEngine);
        
        int timeRemaining = GetSecondsRemaining( playerEngine );
        //HUDController.Instance.ManeuverDisplay.UpdateETADisplay( timeRemaining );

        // Track change, push to ManeuverDisplay
        if ( newPlayerShip.TrackSegment > playerShip.TrackSegment )
        {
            //HUDController.Instance.ManeuverDisplay.TriggerSlide();
        }

        List<Ship.Position> newAiShips = new List<Ship.Position>();
        for (int i = 0; i < ai.aiCount; i++)
		{
            newAiShips.Add(GetNewShipPos(ai.ships[i], ai.engineEfficiencies[i]));
        }

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
}
