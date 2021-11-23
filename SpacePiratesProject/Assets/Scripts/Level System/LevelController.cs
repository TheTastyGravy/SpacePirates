using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : Singleton<LevelController>
{
    public Level level;
    public Ship ship;

    private float playerPos = 0;
    public float PlayerPosition => playerPos;
    private int lastEventIndex = -1;
    private Level.Event currentEvent = null;
    private EventManager eventManager;
    private LevelDificultyData diffData;
    private LevelDificultyData.DiffSetting settings;
    private float timePassed = 0;
    private float endlessLengthAddition = 0;



    void Start()
    {
        Init();
    }

    private void Init()
    {
        diffData = GameManager.DiffData;
        settings = GameManager.GetDifficultySettings();
        level = Level.GetLevel(GameManager.SelectedTrack);
        level.Setup();
        ship = Ship.GetShip(GameManager.SelectedShip);
        Instantiate(ship.ShipPrefab, new Vector3(0, ship.heightOffset, 0), Quaternion.identity);
        // Event for game over
        ShipManager.Instance.OnZeroOxygen += () => OnGameOver(false);

        eventManager = EventManager.Instance;
		eventManager.OnEventChange += OnEventChange;
		TimelineController.Instance.Setup(level);
        TimelineController.Instance.enabled = false;

        foreach (InteractionPromptLogic prompt in FindObjectsOfType<InteractionPromptLogic>())
        {
            prompt.SetHidden(true);
        }

        Invoke(nameof(StartGame), 1);
    }

    private void StartGame()
    {
        TimelineController.Instance.enabled = true;
        ShipManager.Instance.BeginGame();
        AstroidManager.Instance.BeginGame();
        StatusManager.Instance.enabled = true;
        HUDController.Instance.SetDisplayHUD(true);

        foreach (InteractionPromptLogic prompt in FindObjectsOfType<InteractionPromptLogic>())
        {
            prompt.SetHidden(false);
        }

        foreach (Player player in Player.all)
        {
            player.Character.enabled = true;
            (player.Character as Character).IsKinematic = false;
        }
    }

    void Update()
    {
        if (ShipManager.Instance == null)
            return;

        // Update player position
		float playerSpeed = ShipManager.Instance.GetShipSpeed();
        playerPos += playerSpeed * Time.deltaTime;

        if (currentEvent != null)
        {
            eventManager.UpdateEvent();

            // If the player has passed the end, stop the event
            if (playerPos >= currentEvent.end)
            {
				currentEvent = null;
				lastEventIndex++;
				eventManager.StopEvent();
                if (level.isEndless)
                {
                    level.CreateEvent(Random.Range(settings.minEventLength, settings.maxEventLength));
                    TimelineController.Instance.UpdateTimeline();
                    EventManager.Instance.UpdateValues();
                }
            }
        }
        // If there are more events in the level
        else if (level.events.Count > lastEventIndex + 1)
        {
            // If the player has passed the start, start the event
            Level.Event nextEvent = level.events[lastEventIndex + 1];
            if (playerPos >= nextEvent.start)
            {
                currentEvent = nextEvent;
                eventManager.StartEvent(nextEvent);
            }
        }

        if (level.isEndless)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= diffData.timeToDiffIncrease)
            {
                timePassed = 0;
                // Increase difficulty
                endlessLengthAddition += diffData.eventLengthAddition;
                level.eventArea += diffData.eventLengthAddition;


                AstroidManager.Instance.IncreaseDifficulty();
            }
        }

        // Check if the player has reached the end of the level
        if (playerPos >= level.length && !level.isEndless)
		{
            OnGameOver(true);
        }
    }

	private void OnEventChange(Level.Event.Type eventType, EventManager.EventStage stage)
	{
		// If the event has ended early, resize the event to bring the next event forward
		if (stage == EventManager.EventStage.END && currentEvent != null)
		{
			currentEvent = null;
			lastEventIndex++;
			level.ResizeEvent(lastEventIndex, playerPos);
            if (level.isEndless)
            {
                level.CreateEvent(Random.Range(settings.minEventLength, settings.maxEventLength));
            }
			TimelineController.Instance.UpdateTimeline();
		}
	}

    private void OnGameOver(bool hasWon)
	{
        GameManager.SetGameOverInfo(hasWon);

        // Freeze players
        foreach (Player player in Player.all)
		{
            player.Character.enabled = false;
            (player.Character as Character).IsKinematic = true;
            player.Character.GetComponent<Interactor>().enabled = false;
        }
        // Stop everything
        enabled = false;
        AstroidManager.Instance.enabled = false;
        StatusManager.Instance.enabled = false;
        ShipManager.Instance.enabled = false;
        // Fade ship to normal
        ShipManager.Instance.StartCoroutine(ShipManager.Instance.FadeShip(0.75f, false));

        if (hasWon)
		{
            ShipManager.Instance.MoveForward(2, 50);
        }
		else
		{
            ShipManager.Instance.ExplodeShip();
        }

        Invoke(nameof(ChangeScene), 2);
    }

    private void ChangeScene()
	{
        GameManager.ChangeState(GameManager.GameState.SUMMARY);
    }
}
