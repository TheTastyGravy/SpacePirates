using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Player.PlayerSlot PlayerSlot;
    public bool DestroyOnTeleport;
    
    void Awake()
    {
        Player player = Player.GetPlayerBySlot( PlayerSlot );

        if ( player != null )
        {
            ( player.Character as Character).IsKinematic = true;
            player.transform.SetPositionAndRotation( transform.position, transform.rotation );
            player.transform.parent = transform.parent;
            StartCoroutine( GravityTimer( player ) );
        }

        if ( DestroyOnTeleport )
        {
            Destroy( gameObject );
        }
    }

    private IEnumerator GravityTimer( Player a_Player )
    {
        yield return new WaitForSeconds( 0.05f );

        ( a_Player.Character as Character ).IsKinematic = false;
    }
}