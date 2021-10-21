using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipManager : Singleton<ShipManager>
{
    [Header("Oxygen")]
    public float maxOxygenLevel = 100;
    public float passiveOxygenLoss = 1;
    public float timeToGameOver = 0.5f;
    [Header("Avoidance")]
    [Range(0,1)]
    public float maxAvoidance = 0.25f;


    private ReactorStation[] reactors;
    private EngineStation[] engines;
    private RoomManager[] rooms;

    private float oxygenLevel;
    public float OxygenLevel => oxygenLevel;
    private OxygenBar oxygenBar;

    private float gameOverTimmer;

    private Player player;
    private Player.Control[] cheatCode;
    private int cheatIndex = 0;
    private bool cheatActive = false;



    void Start()
    {
        reactors = GetComponentsInChildren<ReactorStation>();
        engines = GetComponentsInChildren<EngineStation>();
        rooms = GetComponentsInChildren<RoomManager>();

        oxygenLevel = maxOxygenLevel;
        oxygenBar = FindObjectOfType<OxygenBar>();
        oxygenBar.MaxValue = maxOxygenLevel;

        // Setup input callbacks for cheat code
        player = Player.GetPlayerBySlot(Player.PlayerSlot.P1);
        player.AddInputListener(Player.Control.DPAD_PRESSED, OnP1Input);
        player.AddInputListener(Player.Control.B_PRESSED, OnP1Input);
        player.AddInputListener(Player.Control.A_PRESSED, OnP1Input);
        player.AddInputListener(Player.Control.START_PRESSED, OnP1Input);
        // Illegal info
        cheatCode = new Player.Control[]
        {
            Player.Control.COUNT + 0,
            Player.Control.COUNT + 0,
            Player.Control.COUNT + 2,
            Player.Control.COUNT + 2,
            Player.Control.COUNT + 3,
            Player.Control.COUNT + 1,
            Player.Control.COUNT + 3,
            Player.Control.COUNT + 1,
            Player.Control.B_PRESSED,
            Player.Control.A_PRESSED,
            Player.Control.START_PRESSED
        };
    }

	void OnDestroy()
	{
        player.RemoveInputListener(Player.Control.DPAD_PRESSED, OnP1Input);
        player.RemoveInputListener(Player.Control.B_PRESSED, OnP1Input);
        player.RemoveInputListener(Player.Control.A_PRESSED, OnP1Input);
        player.RemoveInputListener(Player.Control.START_PRESSED, OnP1Input);
    }

	public void DamageShipAtPosition(Vector3 position)
    {
        HoleData holeData = new HoleData
        {
            hitPos = position,
            sqrDistance = Mathf.Infinity
        };

        // Find the closest hole positon, then damage the room
        foreach (var room in rooms)
        {
            room.FindClosestHolePos(ref holeData);
        }
        holeData.room.DamageRoom(holeData);
    }

    public float GetShipSpeed()
	{
        // Its possible for this function to be called before start
        if (engines == null)
            return 0;

        // Accumulate speed from engines
        float speed = 0;
        foreach (var engine in engines)
		{
            speed += engine.CurrentSpeed;
		}

        return speed;
	}

    public float GetShipAvoidance()
	{
        int active = 0;
        foreach (var obj in engines)
		{
            if (obj.IsTurnedOn)
                active++;
		}

        return maxAvoidance * (active / engines.Length);
	}

    public DamageStation GetRandomActiveStation()
	{
        List<DamageStation> stations = new List<DamageStation>();
        // Add active reactors
        foreach (var obj in reactors)
		{
            if (obj.IsTurnedOn)
			{
                stations.Add(obj.Damage);
			}
		}
        // Add active engines
        foreach (var obj in engines)
		{
            if (obj.IsTurnedOn)
			{
                stations.Add(obj.Damage);
			}
		}

        if (stations.Count > 0)
		{
            return stations[Random.Range(0, stations.Count)];
        }
		else
		{
            return null;
		}
	}


    void Update()
    {
        UpdateOxygen();
    }

    private void UpdateOxygen()
	{
        if (cheatActive)
		{
            //hacker man
            oxygenLevel = 100;
            oxygenBar.value = oxygenLevel;
            return;
        }

        // Find the total loss rate
        float oxygenLossRate = passiveOxygenLoss;
        foreach (var room in rooms)
        {
            oxygenLossRate += room.localOxygenDrain;
        }
        // Decrease level by loss rate
        oxygenLevel -= oxygenLossRate * Time.deltaTime;

        // Find the regen rate and apply it
        float oxygenRegenRate = 0;
        foreach (var reactor in reactors)
		{
            oxygenRegenRate += reactor.CurrentOxygenRegen;
		}
        oxygenLevel += oxygenRegenRate * Time.deltaTime;
        if (oxygenLevel > maxOxygenLevel)
            oxygenLevel = maxOxygenLevel;

        if (oxygenLevel <= 0)
        {
            oxygenLevel = 0;
            // Start timmer to game over
            gameOverTimmer += Time.deltaTime;
            if (gameOverTimmer >= timeToGameOver)
			{
                GameManager.SetGameOverInfo(false);
                GameManager.ChangeState(GameManager.GameState.SUMMARY);
			}
        }
		else
		{
            gameOverTimmer = 0;
		}

        // Update the UI
        oxygenBar.value = oxygenLevel;
    }

    private void OnP1Input(InputAction.CallbackContext context)
    {
        // Get action, correcting for dpad
        InputAction correctAction = player.GetInputAction(cheatCode[cheatIndex] >= Player.Control.COUNT ? Player.Control.DPAD_PRESSED : cheatCode[cheatIndex]);
        if (context.action == correctAction)
		{
            // If its a direction, check dpad input
            if (cheatCode[cheatIndex] >= Player.Control.COUNT)
			{
                Vector2 val = context.ReadValue<Vector2>();
                if (!((val.y == 1 && cheatCode[cheatIndex] == Player.Control.COUNT + 0) ||//up
                    (val.x == 1 && cheatCode[cheatIndex] == Player.Control.COUNT + 1) ||  //right
                    (val.y == -1 && cheatCode[cheatIndex] == Player.Control.COUNT + 2) || //down
                    (val.x == -1 && cheatCode[cheatIndex] == Player.Control.COUNT + 3)))  //left
				{
                    // Fail
                    cheatIndex = 0;
                    return;
				}
            }

            // Continue with code
            cheatIndex++;
            if (cheatIndex == cheatCode.Length)
			{
                cheatActive = true;
                Debug.Log("<size=15><color=red>CHEAT ACTIVATED</color></size>");
                player.RemoveInputListener(Player.Control.DPAD_PRESSED, OnP1Input);
                player.RemoveInputListener(Player.Control.B_PRESSED, OnP1Input);
                player.RemoveInputListener(Player.Control.A_PRESSED, OnP1Input);
                player.RemoveInputListener(Player.Control.START_PRESSED, OnP1Input);
            }
        }
		else
		{
            // Fail
            cheatIndex = 0;
        }
    }
}
