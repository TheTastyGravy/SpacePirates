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

        // Something has gone very wrong. ABORT
        if (PlayerSlot == Player.PlayerSlot.P1 && player == null)
        {
            GameManager.ChangeState(GameManager.GameState.START, true);
            return;
        }

        if ( player != null )
        {
            if (player.Character == null)
			{
                GameManager.ChangeState(GameManager.GameState.CHARACTER, true);
			}

            player.transform.localScale = Vector3.one;
            // Enable the character
            player.Character.gameObject.SetActive(true);
            player.Character.enabled = false;
            player.Character.transform.localPosition = Vector3.zero;
            // Menu spam fix
            (player.Character as Character).SetUseCharacterSelectAnimations(false);

            (player.Character as Character).IsKinematic = true;
            player.transform.SetPositionAndRotation( transform.position, transform.rotation );
            player.transform.parent = transform.parent;
            //StartCoroutine( GravityTimer( player ) );
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