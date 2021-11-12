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



	void Start()
    {
        Init();
    }

    private void Init()
    {
        level = Level.GetLevel(GameManager.SelectedTrack);
        level.Setup();
        ship = Ship.GetShip(GameManager.SelectedShip);
        Instantiate(ship.ShipPrefab, new Vector3(0, ship.heightOffset, 0), Quaternion.identity);
        // Event for game over
        ShipManager.Instance.OnZeroOxygen += () => OnGameOver(false);

        eventManager = EventManager.Instance;
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
            OnGameOver(true);
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
