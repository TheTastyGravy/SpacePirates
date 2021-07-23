using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
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

    // ---------- VISUALS ----------
    public Transform cameraTrans;
    public Transform background;

    private Quaternion initCamRot;
    private Vector3 lastCurvePoint;
    private Vector3 lastCurveDir;



    public TrackType[] track;

    public EngineStation[] leftEngines;
    public EngineStation[] rightEngines;
    public EngineStation[] centerEngines;


    private ShipPosition playerShip;

    private AIManager ai;





    void Start()
    {
        ai = GetComponent<AIManager>();

        initCamRot = cameraTrans.rotation;
    }

    void FixedUpdate()
    {
        //get engine efficiency for player
        float playerEngine = GetEngineEfficiency();

        //get new ship positions for all ships
        ShipPosition newPlayerShip = GetNewShipPos(playerShip, playerEngine);
        List<ShipPosition> newAiShips = new List<ShipPosition>();
        for (int i = 0; i < ai.aiCount; i++)
		{
            newAiShips[i] = GetNewShipPos(ai.ships[i], ai.engineEfficiencies[i]);
        }


        //check if the player has any passes
        CheckForPasses(newPlayerShip, newAiShips);

        //update positions
        playerShip = newPlayerShip;
        ai.ships = newAiShips;


        //update ui and visuals
        UpdateVisuals();
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



        //magic


        return 1f;
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


    // Visuals include background and the camera
    private void UpdateVisuals()
	{
        TrackType currentTrack = track[playerShip.trackIndex];

        //default state
        if (currentTrack == TrackType.Straight)
		{
            lastCurvePoint = Vector3.zero;
            lastCurveDir = Vector3.forward;
            cameraTrans.rotation = initCamRot;
            background.forward = Vector3.forward;
            return;
        }


        Vector3 curvePoint;
        // 90 degree track
        if (currentTrack == TrackType.Left90 || currentTrack == TrackType.Right90)
		{
            curvePoint = GetPointOnCurve(playerShip.segmentDist, new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(1,0,1));
        }
        // 45 degree track
		else
		{
            curvePoint = GetPointOnCurve(playerShip.segmentDist, new Vector3(0, 0, 0), new Vector3(0, 0, 0.414f), new Vector3(0.414f, 0, 1));
        }

        //flip for right turns

        Vector3 curveDir = curvePoint - lastCurvePoint;
        curveDir.Normalize();


        //update cam
        Quaternion rot = Quaternion.FromToRotation(lastCurveDir, curveDir);

        rot = Quaternion.LerpUnclamped(Quaternion.identity, rot, 5);

        rot = initCamRot * rot;
        cameraTrans.rotation = rot;

        //update background
        Vector3 vec = Vector3.zero;
        float angle = Vector3.Angle(lastCurveDir, curveDir) * 5 * Mathf.Deg2Rad;

        Debug.Log(angle * Mathf.Rad2Deg);

        vec.x = Vector3.forward.x * Mathf.Cos(angle) - Vector3.forward.z * Mathf.Sin(angle);
        vec.z = Vector3.forward.x * Mathf.Sin(angle) + Vector3.forward.z * Mathf.Cos(angle);
        background.forward = vec;

        lastCurvePoint = curvePoint;
        lastCurveDir = curveDir;
	}


    private Vector3 GetPointOnCurve(float t, in Vector3 p1, in Vector3 p2, in Vector3 p3)
    {
        if (t < 0 || t > 1)
            return Vector3.zero;

        float c = 1.0f - t;

        // The Bernstein polynomials
        float bb0 = c * c;
        float bb1 = 2 * c * t;
        float bb2 = t * t;

        // Return the point
        return p1 * bb0 + p2 * bb1 + p3 * bb2;
    }
}
