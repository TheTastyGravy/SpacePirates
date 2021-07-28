using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TrackManager : Singleton<TrackManager>
{
    public enum TrackType
	{
        Straight,
        Left90,
        Left45,
        Right90,
        Right45
	}

    [System.Serializable]
    public struct ShipPosition
	{
        public int trackIndex;
        public float segmentDist;
	}

    
    public TrackType[] track;
    [Space]
    public EngineStation[] leftEngines;
    public EngineStation[] rightEngines;
	public EngineStation[] centerEngines;
    [Space]
    [Tooltip("Invoked when the player reaches the end of the track")]
    public UnityEvent onPlayerFinish;

    [Header("Camera")]
    public Transform cameraTrans;
    public AnimationCurve curve90;
    public float maxAngle90;
    public AnimationCurve curve45;
    public float maxAngle45;

	[Header("UI")]
	public Text currentTrack;
	public Text nextTrack;
	public Text nextNextTrack;
	[Space]
	public Text nextShip;
	public Text previousShip;


    private ShipPosition playerShip;
    private AIManager ai;

	private string currentTrackBase;
	private string nextTrackBase;
	private string nextNextTrackBase;
	private string nextShipBase;
	private string previousShipBase;



    void Start()
    {
		ai = AIManager.Instance;

		currentTrackBase = currentTrack.text;
		nextTrackBase = nextTrack.text;
		nextNextTrackBase = nextNextTrack.text;
		nextShipBase = nextShip.text;
		previousShipBase = previousShip.text;
	}

    void Update()
    {
        // Get engine efficiency for player
        float playerEngine = GetEngineEfficiency();

        // Get new ship positions
        ShipPosition newPlayerShip = GetNewShipPos(playerShip, playerEngine);
        List<ShipPosition> newAiShips = new List<ShipPosition>();
        for (int i = 0; i < ai.aiCount; i++)
		{
            newAiShips.Add(GetNewShipPos(ai.ships[i], ai.engineEfficiencies[i]));
        }

        // Check if the player has any passes
        CheckForPasses(newPlayerShip, newAiShips);

        // Update positions
        playerShip = newPlayerShip;
        ai.ships = newAiShips;
        // Check if the player has reached the end of the track
        if (playerShip.trackIndex == track.Length-1 && playerShip.segmentDist == 1)
		{
            onPlayerFinish.Invoke();
        }

        UpdateCamera();
        UpdateUI();
    }


    private float GetEngineEfficiency()
	{
        float left = 0;
        float right = 0;
        float center = 0;

        #region EngineAverages
        if (leftEngines.Length == 0)
        {
            left = 2;
        }
        else
        {
            foreach (var obj in leftEngines)
            {
                left += obj.PowerLevel;
            }
            left /= leftEngines.Length;
        }
        if (rightEngines.Length == 0)
        {
            right = 2;
        }
        else
        {
            foreach (var obj in rightEngines)
            {
                right += obj.PowerLevel;
            }
            right /= rightEngines.Length;
        }
        if (centerEngines.Length == 0)
        {
            center = 2;
        }
        else
        {
            foreach (var obj in centerEngines)
            {
                center += obj.PowerLevel;
            }
            center /= centerEngines.Length;
        }
        #endregion


        float power = 0;

        //  THIS NEEDS TO BE GENERALISED
        TrackType currentTrack = track[playerShip.trackIndex];
        switch (currentTrack)
		{
            case TrackType.Straight:
				if (left == right)
				{
                    //divide by 9 because 3 regions * 3 power levels
                    power = (left + right + center) / 9f;
				}
                break;

            case TrackType.Left90:
                if (left == 1 && right == 3)
                    power = 1;
                else if (left == 2 && right == 3)
                    power = 0.666f;
                else if (left == 1 && right == 2)
                    power = 0.333f;
                break;
            case TrackType.Right90:
                if (left == 3 && right == 1)
                    power = 1;
                else if (left == 3 && right == 2)
                    power = 0.666f;
                else if (left == 2 && right == 1)
                    power = 0.333f;
                break;

            case TrackType.Left45:
                if (left == 1 && right == 3)
                    power = 0.333f;
                else if (left == 2 && right == 3)
                    power = 1;
                else if (left == 1 && right == 2)
                    power = 0.666f;
                break;
            case TrackType.Right45:
                if (left == 3 && right == 1)
                    power = 0.333f;
                else if (left == 3 && right == 2)
                    power = 1;
                else if (left == 2 && right == 1)
                    power = 0.666f;
                break;
        }


        //min value
        if (power < 0.25f)
            power = 0.25f;
        return power;
	}

    private ShipPosition GetNewShipPos(ShipPosition shipPos, float engineEfficiency)
	{
        // Update pos, looping segment dist into track index
        shipPos.segmentDist += engineEfficiency * Time.deltaTime;
        if (shipPos.segmentDist > 1)
		{
            shipPos.segmentDist -= 1;
            shipPos.trackIndex++;
		}
        // Prevent ship position from going past the end of the track
        if (shipPos.trackIndex >= track.Length)
		{
            shipPos.segmentDist = 1;
            shipPos.trackIndex = track.Length - 1;
        }

        return shipPos;
	}

    private void CheckForPasses(in ShipPosition newPlayerShip, in List<ShipPosition> newAiShips)
	{
        for (int i = 0; i < ai.aiCount; i++)
		{
            //determine last state
            bool wasBehind = true;
            if (playerShip.trackIndex < ai.ships[i].trackIndex)
			{
                wasBehind = false;
			}
            else if (playerShip.trackIndex == ai.ships[i].trackIndex)
			{
                wasBehind = playerShip.segmentDist > ai.ships[i].segmentDist;
			}
            //determine current state
            bool isBehind = true;
            if (newPlayerShip.trackIndex < newAiShips[i].trackIndex)
			{
                isBehind = false;
			}
            else if (newPlayerShip.trackIndex == newAiShips[i].trackIndex)
			{
                isBehind = newPlayerShip.segmentDist > newAiShips[i].segmentDist;
			}


            if (isBehind != wasBehind)
			{
                if (isBehind)
				{
                    //player passed ai
				}
				else
				{
                    //ai passed player
				}
			}
		}
	}


    private void UpdateCamera()
	{
        TrackType currentTrack = track[playerShip.trackIndex];

        //default state
        if (currentTrack == TrackType.Straight)
		{
            cameraTrans.forward = Vector3.forward;
            return;
        }


        float angle;
        // 90 degree track
        if (currentTrack == TrackType.Left90 || currentTrack == TrackType.Right90)
		{
            angle = curve90.Evaluate(playerShip.segmentDist) * maxAngle90;
        }
        // 45 degree track
		else
		{
            angle = curve45.Evaluate(playerShip.segmentDist) * maxAngle45;
        }
        angle *= Mathf.Deg2Rad;

        // Get direction from angle
        Vector3 dir = Vector3.zero;
        dir.x = Vector3.forward.x * Mathf.Cos(angle) - Vector3.forward.z * Mathf.Sin(angle);
        dir.z = Vector3.forward.x * Mathf.Sin(angle) + Vector3.forward.z * Mathf.Cos(angle);
        // Flip direction for left turns
        if (currentTrack == TrackType.Left90 || currentTrack == TrackType.Left45)
            dir.x = -dir.x;
		
        cameraTrans.forward = dir;
	}

    private void UpdateUI()
	{
		//display track info
		currentTrack.text = currentTrackBase + Track2String(track[playerShip.trackIndex]);
		if (playerShip.trackIndex + 1 < track.Length)
		{
			nextTrack.text = nextTrackBase + Track2String(track[playerShip.trackIndex + 1]) + "\nIn " + (1 - playerShip.segmentDist);
		}
		if (playerShip.trackIndex + 2 < track.Length)
		{
			nextNextTrack.text = nextNextTrackBase + Track2String(track[playerShip.trackIndex + 2]) + "\nIn " + (2 - playerShip.segmentDist);
		}

		//get closest opponents
		float playerDist = playerShip.trackIndex + playerShip.segmentDist;

		float nextOpponent = float.PositiveInfinity;
		float prevOpponent = float.NegativeInfinity;
		for (int i = 0; i < ai.aiCount; i++)
		{
			float dist = ai.ships[i].trackIndex + ai.ships[i].segmentDist;
			dist -= playerDist;

			if (dist > 0) //ahead of player
			{
				if (nextOpponent > dist)
					nextOpponent = dist;
			}
			else //behind player
			{
				if (prevOpponent < dist)
					nextOpponent = dist;
			}
		}
		//display opponent info
		nextShip.text = nextShipBase + nextOpponent.ToString();
		nextShip.text = previousShipBase + prevOpponent.ToString();
	}
	private string Track2String(TrackType track)
	{
		switch (track)
		{
			case TrackType.Straight:
				return "Straight";
			case TrackType.Left90:
				return "90 Left";
			case TrackType.Left45:
				return "45 Left";
			case TrackType.Right90:
				return "90 Right";
			case TrackType.Right45:
				return "45 Right";

			default:
				return "Unknown Track Type";
		}
	}
}
