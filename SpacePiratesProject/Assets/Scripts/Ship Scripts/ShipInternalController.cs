using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInternalController : MonoBehaviour
{
    public Transform shipExternal;
    public GameObject[] players;

    private Rigidbody rb;
    private Rigidbody shipRb;
    private Rigidbody[] playerRbs;

    // The ships external rotation last update
    private Quaternion lastRot;


	void Start()
	{
        rb = GetComponent<Rigidbody>();
        shipRb = shipExternal.GetComponent<Rigidbody>();

        playerRbs = new Rigidbody[players.Length];
        for (int i = 0; i < players.Length; i++)
		{
            playerRbs[i] = players[i].GetComponent<Rigidbody>();
		}


        lastRot = shipRb.rotation;

        // Reset physics values for the ship internal, just in case
        rb.ResetCenterOfMass();
        rb.ResetInertiaTensor();
    }

	void FixedUpdate()
    {
        // Move the internals by the rigidbody directly
        rb.position = shipRb.position;
        rb.rotation = shipRb.rotation;

        // Move each player
        for (int i = 0; i < playerRbs.Length; i++)
		{
            // Just set the rotation. When players are able to rotate, this will need to be changed
            playerRbs[i].rotation = shipRb.rotation;

            // The offset is the difference in position between the player and the ship, plus how far the ship has moved last frame
            Vector3 offset = (playerRbs[i].position - shipRb.position) + shipRb.velocity * Time.fixedDeltaTime;
            // Rotate the offset by the change in rotation this update
            Quaternion deltaQuaternion = Quaternion.Inverse(shipRb.rotation) * lastRot;
            offset = Quaternion.Inverse(deltaQuaternion) * offset;
            // Move the player
            playerRbs[i].MovePosition(shipRb.position + offset);
        }

        // Update the stored rotation
        lastRot = shipRb.rotation;
    }
}
