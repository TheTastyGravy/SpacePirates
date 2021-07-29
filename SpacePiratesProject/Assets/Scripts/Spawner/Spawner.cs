using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Player.PlayerSlot PlayerSlot;
    public bool DestroyOnTeleport;
    
    void Start()
    {
        Player player = Player.GetPlayerBySlot( PlayerSlot );

        if ( player == null )
        {
            return;
        }

        player.transform.SetPositionAndRotation( transform.position, transform.rotation );
        player.transform.parent = transform.parent;

        if ( DestroyOnTeleport )
        {
            Destroy( gameObject );
        }
    }
}