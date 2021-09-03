using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : Singleton<LevelController>
{
    public Level level;
    public Ship ship;

    private float playerPos = 0;
    private int lastEventIndex = -1;
    private Level.Event currentEvent = null;
    private EventManager eventManager;



	void Start()
    {
        level = Level.GetLevel( GameManager.SelectedTrack );
        ship = Ship.GetShip( GameManager.SelectedShip );
        Instantiate( ship.ShipPrefab );

        eventManager = EventManager.Instance;
    }

    void Update()
    {
        // Update player position
		float playerSpeed = ShipManager.Instance.GetShipSpeed();
        playerPos += playerSpeed * Time.deltaTime;

        if (currentEvent != null)
        {
            eventManager.UpdateEvent();

            // If the player has passed the end, stop the event
            if (playerPos >= currentEvent.end)
            {
                eventManager.StopEvent();
                lastEventIndex++;
                currentEvent = null;
            }
        }
        // If there are more events in the level
        else if (level.events.Length > lastEventIndex + 1)
        {
            // If the player has passed the start, start the event
            Level.Event nextEvent = level.events[lastEventIndex + 1];
            if (playerPos >= nextEvent.start)
            {
                currentEvent = nextEvent;
                eventManager.StartEvent(nextEvent);
            }
        }

        // Check if the player has reached the end of the level
        if (playerPos >= level.length)
		{
            //win state
            GameManager.CurrentState = GameManager.GameState.SUMMARY;
        }
    }
}
